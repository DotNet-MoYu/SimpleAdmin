using Microsoft.AspNetCore.Authentication;
using NewLife.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IMqttService"/>
/// </summary>
public class MqttService : IMqttService
{
    private readonly IMqttClientManager _mqttClientManager;
    private readonly ISimpleRedis _simpleRedis;
    private readonly ISysUserService _sysUserService;
    private readonly IConfigService _configService;
    public MqttService(ISimpleRedis simpleRedis, ISysUserService sysUserService, IConfigService configService)
    {

        this._simpleRedis = simpleRedis;
        this._sysUserService = sysUserService;
        this._configService = configService;

    }

    /// <inheritdoc/>
    public async Task<MqttParameterOutput> GetParameter()
    {
        var token = JWTEncryption.GetJwtBearerToken((DefaultHttpContext)App.HttpContext); // 获取当前token
        //获取mqtt配置
        var mqttconfig = await _configService.GetListByCategory(CateGoryConst.Config_MQTT_BASE);
        //地址
        var url = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_URL).Select(it => it.ConfigValue).FirstOrDefault();
        //用户名
        var userName = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_USERNAME).Select(it => it.ConfigValue).FirstOrDefault();
        //密码
        var password = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_PASSWORD).Select(it => it.ConfigValue).FirstOrDefault();
        #region 用户名特殊处理
        if (userName.ToLower() == "$username")
            userName = UserManager.UserAccount;
        else if (userName.ToLower() == "$userid")
            userName = UserManager.UserId.ToString();
        #endregion
        #region 密码特殊处理
        if (password.ToLower() == "$username")
            password = token; // 当前token作为mqtt密码
        #endregion
        var clientId = $"{UserManager.UserId}_{RandomHelper.CreateLetterAndNumber(5)}";//客户端ID
        _simpleRedis.Set(RedisConst.Redis_MqttClientUser + clientId, token, TimeSpan.FromMinutes(1));//将该客户端ID对应的token插入redis后面可以根据这个判断是哪个token登录的
        return new MqttParameterOutput
        {
            ClientId = clientId,
            Password = password,
            Url = url,
            UserName = userName,
            Topics = new List<string> { MqttConst.Mqtt_TopicPrefix + UserManager.UserId }//默认监听自己
        };
    }

    /// <inheritdoc/>
    public async Task<MqttAuthOutput> Auth(MqttAuthInput input)
    {
        MqttAuthOutput mqttAuthOutput = new MqttAuthOutput { Is_superuser = false, Result = "deny" };
        var user = await _sysUserService.GetUserByAccount(input.Username);//获取用户信息
        if (user != null)
        {
            //获取用户token
            var tokens = _simpleRedis.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserToken, user.Id.ToString());
            if (tokens != null)
            {
                if (tokens.Any(it => it.Token == input.Password))//判断是否有token
                    mqttAuthOutput.Result = "allow";//允许登录
            }
        }
        return mqttAuthOutput;
    }

    /// <inheritdoc/>
    public async Task LoginOut(string userId, List<string> clientIds, string message)
    {
        var mqttClientManager = App.GetService<IMqttClientManager>();//获取mqtt服务
        //发送通知下线消息
        await mqttClientManager.GetClient().PublishAsync(MqttConst.Mqtt_TopicPrefix + userId, new MqttMessage
        {
            Data = new { Message = message, ClientIds = clientIds },
            MsgType = MqttConst.Mqtt_Message_LoginOut
        });
    }

}
