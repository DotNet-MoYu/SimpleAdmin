// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Furion.DataEncryption;
using Microsoft.AspNetCore.Http;
using SimpleTool;

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// <inheritdoc cref="IMqttService"/>
/// </summary>
public class MqttService : IMqttService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISysUserService _sysUserService;
    private readonly IConfigService _configService;

    public MqttService(ISimpleCacheService simpleCacheService, ISysUserService sysUserService, IConfigService configService)
    {
        _simpleCacheService = simpleCacheService;
        _sysUserService = sysUserService;
        _configService = configService;
    }

    /// <inheritdoc/>
    public async Task<MqttParameterOutput> GetWebLoginParameter()
    {
        var user = await _sysUserService.GetUserById(UserManager.UserId);//获取用户信息
        var token = JWTEncryption.GetJwtBearerToken((DefaultHttpContext)App.HttpContext);// 获取当前token
        //获取mqtt配置
        var mqttconfig = await _configService.GetListByCategory(CateGoryConst.Config_MQTT_BASE);
        //地址
        var url = mqttconfig.Where(it => it.ConfigKey == SysConfigConst.MQTT_PARAM_URL).Select(it => it.ConfigValue).FirstOrDefault();
        //用户名
        var userName = mqttconfig.Where(it => it.ConfigKey == SysConfigConst.MQTT_PARAM_USERNAME).Select(it => it.ConfigValue).FirstOrDefault();
        //密码
        var password = mqttconfig.Where(it => it.ConfigKey == SysConfigConst.MQTT_PARAM_PASSWORD).Select(it => it.ConfigValue).FirstOrDefault();

        #region 用户名特殊处理

        if (userName.ToLower() == "$username")
            userName = user.Account;
        else if (userName.ToLower() == "$userid")
            userName = user.Id.ToString();

        #endregion 用户名特殊处理

        #region 密码特殊处理

        if (password.ToLower() == "$username")
            password = token;// 当前token作为mqtt密码

        #endregion 密码特殊处理

        var clientId = $"{user.Id}_{RandomHelper.CreateLetterAndNumber(5)}";//客户端ID
        _simpleCacheService.Set(MqttConst.Cache_MqttClientUser + clientId, token, TimeSpan.FromMinutes(1));//将该客户端ID对应的token插入redis后面可以根据这个判断是哪个token登录的
        return new MqttParameterOutput
        {
            ClientId = clientId,
            Password = password,
            Url = url,
            UserName = userName,
            Topics = new List<string> { MqttConst.Mqtt_TopicPrefix + user.Id }//默认监听自己
        };
    }

    /// <inheritdoc/>
    public async Task<MqttAuthOutput> Auth(MqttAuthInput input)
    {
        var user = await _sysUserService.GetUserByAccount(input.Username);
        var mqttAuthOutput = new MqttAuthOutput { Is_superuser = false, Result = "deny" };

        //获取用户token
        var tokens = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, user.Id.ToString());
        if (tokens != null)
        {
            if (tokens.Any(it => it.Token == input.Password))//判断是否有token
                mqttAuthOutput.Result = "allow";//允许登录
        }
        return mqttAuthOutput;
    }
}