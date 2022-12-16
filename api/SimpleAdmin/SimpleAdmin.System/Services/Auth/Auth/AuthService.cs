using IPTools.Core.Extensions;
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
            return await ExecLoginB(userInfo, input.Device, loginClientType);// 执行B端登录
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
            var result = await ExecLoginB(userInfo, input.Device, loginClientType);// 执行B端登录
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
    public async Task LoginOut(string token, LoginClientTypeEnum loginClientType)
    {
        //获取用户信息
        var userinfo = await _userService.GetUserByAccount(UserManager.UserAccount);
        if (userinfo != null)
        {
            LoginEvent loginEvent = new LoginEvent
            {
                IpInfo = App.HttpContext.GetRemoteIpInfo(),
                SysUser = userinfo,
                Token = token
            };
            RemoveTokenFromRedis(loginEvent, loginClientType);//移除token
            //发布登出事件总线
            await _eventPublisher.PublishAsync(EventSubscriberConst.LoginOutB, loginEvent);
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
    public async Task<LoginOutPut> ExecLoginB(SysUser sysUser, AuthDeviceTypeEumu device, LoginClientTypeEnum loginClientType)
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
        //var httpContext = App.HttpContext;
        //var client = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);
        //登录事件参数
        var logingEvent = new LoginEvent
        {
            IpInfo = App.HttpContext.GetRemoteIpInfo(),
            Device = device,
            Expire = expire,
            SysUser = sysUser,
            Token = accessToken
        };
        await WriteTokenToRedis(logingEvent, loginClientType);//写入token到redis
        await _eventPublisher.PublishAsync(EventSubscriberConst.LoginB, logingEvent); //发布登录事件总线
        //返回结果
        return new LoginOutPut { Token = accessToken, Account = sysUser.Account, Name = sysUser.Name };
    }

    /// <summary>
    /// 写入用户token到redis
    /// </summary>
    /// <param name="loginEvent">登录事件参数</param>
    /// <param name="loginClientType">登录类型</param>
    private async Task WriteTokenToRedis(LoginEvent loginEvent, LoginClientTypeEnum loginClientType)
    {
        var key = loginClientType == LoginClientTypeEnum.B ? RedisConst.Redis_UserTokenB : RedisConst.Redis_UserTokenC;
        //获取token列表
        List<TokenInfo> tokenInfos = GetTokenInfos(loginEvent.SysUser.Id);
        var tokenTimeout = loginEvent.DateTime.AddMinutes(loginEvent.Expire);
        //生成token信息
        var tokenInfo = new TokenInfo
        {
            Device = loginEvent.Device.ToString(),
            Expire = loginEvent.Expire,
            TokenTimeout = tokenTimeout,
            Token = loginEvent.Token,
        };
        bool isSingle = false;//默认不开启单用户登录
        var singleConfig = await _configService.GetByConfigKey(CateGoryConst.Config_SYS_BASE, DevConfigConst.SYS_DEFAULT_SINGLE_OPEN);//获取系统单用户登录选项
        if (singleConfig != null) isSingle = singleConfig.ConfigValue.ToBoolean();//如果配置不为空则设置单用户登录选项为系统配置的值
        //判断是否单用户登录
        if (isSingle)
        {
            tokenInfos = new List<TokenInfo> { tokenInfo };//直接就一个
        }
        else
        {
            //判断是否是空的
            if (tokenInfos != null)
            {
                tokenInfos.Add(tokenInfo);
            }
            else
            {
                tokenInfos = new List<TokenInfo> { tokenInfo };//直接就一个
            }

        }
        //添加到token列表
        _redisCacheManager.HashAdd(key, loginEvent.SysUser.Id.ToString(), tokenInfos);
    }


    /// <summary>
    /// redis删除用户token
    /// </summary>
    /// <param name="loginEvent">登录事件参数</param>
    /// <param name="loginClientType">登录类型</param>
    private void RemoveTokenFromRedis(LoginEvent loginEvent, LoginClientTypeEnum loginClientType)
    {
        var key = loginClientType == LoginClientTypeEnum.B ? RedisConst.Redis_UserTokenB : RedisConst.Redis_UserTokenC;
        //获取token列表
        List<TokenInfo> tokenInfos = GetTokenInfos(loginEvent.SysUser.Id);
        if (tokenInfos != null)
        {
            //获取当前用户的token
            var token = tokenInfos.Where(it => it.Token == loginEvent.Token).FirstOrDefault();
            if (token != null)
                tokenInfos.Remove(token);
            if (tokenInfos.Count > 0)
            {
                //更新token列表
                _redisCacheManager.HashAdd(key, loginEvent.SysUser.Id.ToString(), tokenInfos);
            }
            else
            {
                //从列表中删除
                _redisCacheManager.HashDel<List<TokenInfo>>(key, new string[] { loginEvent.SysUser.Id.ToString() });
            }
        }

    }

    /// <summary>
    /// 获取用户token列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>token列表</returns>
    private List<TokenInfo> GetTokenInfos(long userId)
    {
        //redis获取用户token列表
        List<TokenInfo> tokenInfos = _redisCacheManager.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserTokenB, userId.ToString());
        if (tokenInfos != null)
        {
            tokenInfos = tokenInfos.Where(it => it.TokenTimeout > DateTime.Now).ToList();//去掉登录超时的
        }
        return tokenInfos;

    }
    #endregion
}
