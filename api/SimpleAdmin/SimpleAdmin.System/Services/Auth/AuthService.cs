using UAParser;

namespace SimpleAdmin.System.Services.Auth;

/// <inheritdoc cref="IAuthService"/>
public class AuthService : IAuthService
{
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfigService _configService;
    private readonly ISysUserService _userService;
    private readonly IRoleService _roleService;

    public AuthService(IRedisCacheManager redisCacheManager,
                       IEventPublisher eventPublisher,
                       IConfigService configService,
                       ISysUserService userService,
                       IRoleService roleService)
    {
        _redisCacheManager = redisCacheManager;
        this._eventPublisher = eventPublisher;
        _configService = configService;
        _userService = userService;
        this._roleService = roleService;
    }

    /// <inheritdoc/>
    public PicValidCodeOutPut GetCaptchaInfo()
    {
        //生成验证码
        var captchInfo = CaptchaUtil.CreateCaptcha(CaptchaType.CHAR, 4, 100, 38);
        //生成请求号，并将验证码放入redis
        var reqNo = AddValidCodeToRedis(captchInfo.Code);
        //返回验证码和请求号
        return new PicValidCodeOutPut { ValidCodeBase64 = captchInfo.Base64Str, ValidCodeReqNo = reqNo };
    }

    /// <inheritdoc/>
    public async Task<string> GetPhoneValidCode(GetPhoneValidCodeInput input, LoginClientTypeEnum loginClientType)
    {
        await ValidPhoneValidCode(input, loginClientType);//校验手机号验证码
        var phoneValidCode = RandomHelper.CreateNum(6);//生产随机数字
        #region 发送短信和记录数据库等操作
        //这里为什么不封装阿里云或者腾讯云短信接口，是因为短信发送是一个通用功能，一般每个公司都有自己封装好的短信组件来适配各种项目,这里再封装一次显得多余了。
        //这里不建议在该系统中封装短信发送接口，当然后续如果确实有需要也会增加
        //这里执行发送短信验证码代码，这里先默认就是年月日
        phoneValidCode = DateTime.Now.ToString("yyMMdd");
        #endregion
        //生成请求号，并将验证码放入redis
        var reqNo = AddValidCodeToRedis(phoneValidCode);
        return reqNo;
    }

    /// <inheritdoc/>
    public async Task<LoginOutPut> Login(LoginInput input, LoginClientTypeEnum loginClientType)
    {
        //判断是否有验证码
        var sysBase = await _configService.GetByConfigKey(CateGoryConst.Config_SYS_BASE, DevConfigConst.SYS_DEFAULT_CAPTCHA_OPEN);
        if (sysBase != null)//如果有这个配置项
        {
            if (sysBase.ConfigValue.ToBoolean())//如果需要验证码
            {
                //如果没填验证码，提示验证码不能为空
                if (string.IsNullOrEmpty(input.ValidCode) || string.IsNullOrEmpty(input.ValidCodeReqNo)) throw Oops.Bah("验证码不能为空");
                ValidValidCode(input.ValidCode, input.ValidCodeReqNo);//校验验证码
            }
        }
        var password = CryptogramUtil.Sm2Decrypt(input.Password);//SM2解密

        // 根据账号获取用户信息，根据B端或C端判断
        if (loginClientType == LoginClientTypeEnum.B)//如果是B端
        {

            var userInfo = await _userService.GetUserByAccount(input.Account);//获取用户信息
            if (userInfo == null) throw Oops.Bah("用户不存在");//用户不存在
            if (userInfo.Password != password) throw Oops.Bah("账号密码错误");//账号密码错误
            return await ExecLoginB(userInfo, input.Device);// 执行B端登录
        }
        else
        {
            //执行c端登录
            return null;
        }

    }

    /// <inheritdoc/>
    public async Task<LoginOutPut> LoginByPhone(LoginByPhoneInput input, LoginClientTypeEnum loginClientType)
    {
        await ValidPhoneValidCode(input, loginClientType, false);//校验手机号和验证码，这里不删除Redis，防止输入错误又要重新输入验证码
        // 根据手机号获取用户信息，根据B端或C端判断
        if (loginClientType == LoginClientTypeEnum.B)//如果是B端
        {
            var userInfo = await _userService.GetUserByPhone(input.Phone);//获取信息
            if (userInfo == null) throw Oops.Bah("用户不存在");//用户不存在
            var result = await ExecLoginB(userInfo, input.Device);// 执行B端登录
            RemoveValidCodeFromRedis(input.ValidCodeReqNo);//删除验证码
            return result;
        }
        else
        {
            //执行c端登录
            return null;
        }


    }

    /// <inheritdoc/>
    public async Task LoginOut(string token)
    {
        //获取用户信息
        var userinfo = await _userService.GetUserByAccount(UserManager.UserAccount);
        if (userinfo != null)
        {
            var httpContext = App.HttpContext;
            var client = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);//客户端信息
            //发布登出事件总线
            await _eventPublisher.PublishAsync(EventSubscriberConst.LoginOut, new LoginEvent
            {
                ClientInfo = client,
                Ip = httpContext.GetRemoteIpAddressToIPv4(),
                SysUser = userinfo,
                Token = token
            });
        }

    }


    /// <inheritdoc/>
    public async Task<SysUser> GetLoginUser()
    {
        var userInfo = await _userService.GetUserByAccount(UserManager.UserAccount);//根据账号获取用户信息
        if (userInfo != null)
        {
            //去掉部分信息
            userInfo.Password = null;
            userInfo.PermissionCodeList = null;
            userInfo.DataScopeList = null;
            return userInfo;
        }
        return userInfo;
    }

    #region 方法
    /// <summary>
    /// 校验验证码方法
    /// </summary>
    /// <param name="validCode">验证码</param>
    /// <param name="validCodeReqNo">请求号</param>
    /// <param name="isDelete">是否从Redis删除</param>
    public void ValidValidCode(string validCode, string validCodeReqNo, bool isDelete = true)
    {

        var key = RedisConst.Redis_Captcha + validCodeReqNo; //获取验证码Key值
        var code = _redisCacheManager.Get<string>(key);//从redis拿数据
        if (isDelete) RemoveValidCodeFromRedis(validCodeReqNo);//如果需要删除验证码
        if (code != null)//如果有
        {
            //验证码如果不匹配直接抛错误，这里忽略大小写
            if (validCode.ToLower() != code.ToLower()) throw Oops.Bah("验证码错误");
        }
        else
        {
            throw Oops.Bah("验证码不能为空");//抛出验证码不能为空
        }
    }

    /// <summary>
    /// 从Redis中删除验证码
    /// </summary>
    /// <param name="validCodeReqNo"></param>
    public void RemoveValidCodeFromRedis(string validCodeReqNo)
    {
        var key = RedisConst.Redis_Captcha + validCodeReqNo; //获取验证码Key值
        _redisCacheManager.Remove(key);//删除验证码
    }

    /// <summary>
    /// 校验手机验证码
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <param name="loginClientType">登录端类型</param>
    /// <param name="isDelete">是否删除</param>
    /// <returns></returns>
    public async Task ValidPhoneValidCode(GetPhoneValidCodeInput input, LoginClientTypeEnum loginClientType, bool isDelete = true)
    {
        ValidValidCode(input.ValidCode, input.ValidCodeReqNo, isDelete);//校验验证码
        if (loginClientType == LoginClientTypeEnum.B)//B端登录
        {
            var userId = await _userService.GetIdByPhone(input.Phone);//获取用户名
            if (userId == 0)
            {
                throw Oops.Bah("手机号不存在");//手机号不存在
            }
        }
        else
        {
            //校验C段用户手机号
            throw Oops.Bah("手机号不存在");//抛出验证码不能为空
        }
    }


    /// <summary>
    /// 添加验证码到redis
    /// </summary>
    /// <param name="code">验证码</param>
    /// <param name="expire">过期时间</param>
    /// <returns>验证码请求号</returns>
    public string AddValidCodeToRedis(string code, int expire = 5)
    {
        //生成请求号
        var reqNo = YitIdHelper.NextId().ToString();
        //插入redis
        _redisCacheManager.Set(RedisConst.Redis_Captcha + reqNo, code, TimeSpan.FromMinutes(expire));
        return reqNo;
    }

    /// <summary>
    /// 执行B端登录
    /// </summary>
    /// <param name="sysUser"></param>
    /// <param name="device"></param>
    /// <returns></returns>
    public async Task<LoginOutPut> ExecLoginB(SysUser sysUser, AuthDeviceTypeEumu device)
    {
        if (sysUser.UserStatus == DevDictConst.COMMON_STATUS_DISABLED) throw Oops.Bah("账号已停用");//账号冻结

        //生成Token
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
        {
            {ClaimConst.UserId, sysUser.Id},
            {ClaimConst.Account, sysUser.Account},
            {ClaimConst.Name, sysUser.Name},
            {ClaimConst.IsSuperAdmin, sysUser.RoleCodeList.Contains(RoleConst.SuperAdmin)},
            {ClaimConst.OrgId, sysUser.OrgId},
        });
        var expire = App.GetConfig<int>("JWTSettings:ExpiredTime");//获取过期时间(分钟)
        var expirtTime = DateTime.UtcNow.AddMinutes(expire).ToLong();
        // 生成刷新Token令牌
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, expire * 2);
        // 设置Swagger自动登录
        App.HttpContext.SigninToSwagger(accessToken);
        // 设置响应报文头
        App.HttpContext.SetTokensOfResponseHeaders(accessToken, refreshToken);
        var httpContext = App.HttpContext;
        var client = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);
        //发布登录事件总线
        await _eventPublisher.PublishAsync(EventSubscriberConst.Login, new LoginEvent
        {
            ClientInfo = client,
            Ip = httpContext.GetRemoteIpAddressToIPv4(),
            Device = device,
            LoginClientType = LoginClientTypeEnum.B,
            Expire = expire,
            SysUser = sysUser,
            Token = accessToken
        });
        //返回结果
        return new LoginOutPut { Token = accessToken, Account = sysUser.Account, Name = sysUser.Name };
    }
    #endregion
}
