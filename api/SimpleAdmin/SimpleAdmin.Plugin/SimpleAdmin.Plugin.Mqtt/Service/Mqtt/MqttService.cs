using Furion.DataEncryption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NewLife.MQTT;
using SimpleRedis;
using SimpleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// <inheritdoc cref="IMqttService"/>
/// </summary>
public class MqttService : IMqttService
{
    private readonly ISimpleRedis _simpleRedis;

    public MqttService(ISimpleRedis simpleRedis)
    {

        this._simpleRedis = simpleRedis;

    }

    /// <inheritdoc/>
    public async Task<MqttParameterOutput> GetWebLoginParameter(SysUser user)
    {
        var token = JWTEncryption.GetJwtBearerToken((DefaultHttpContext)App.HttpContext); // 获取当前token
        //获取mqtt配置
        var mqttconfig = await GetMqttConfig();
        //地址
        var url = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_URL).Select(it => it.ConfigValue).FirstOrDefault();
        //用户名
        var userName = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_USERNAME).Select(it => it.ConfigValue).FirstOrDefault();
        //密码
        var password = mqttconfig.Where(it => it.ConfigKey == DevConfigConst.MQTT_PARAM_PASSWORD).Select(it => it.ConfigValue).FirstOrDefault();
        #region 用户名特殊处理
        if (userName.ToLower() == "$username")
            userName = user.Account;
        else if (userName.ToLower() == "$userid")
            userName = user.Id.ToString();
        #endregion
        #region 密码特殊处理
        if (password.ToLower() == "$username")
            password = token; // 当前token作为mqtt密码
        #endregion
        var clientId = $"{user.Id}_{RandomHelper.CreateLetterAndNumber(5)}";//客户端ID
        _simpleRedis.Set(RedisConst.Redis_MqttClientUser + clientId, token, TimeSpan.FromMinutes(1));//将该客户端ID对应的token插入redis后面可以根据这个判断是哪个token登录的
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
    public async Task<MqttAuthOutput> Auth(MqttAuthInput input, string userId)
    {
        MqttAuthOutput mqttAuthOutput = new MqttAuthOutput { Is_superuser = false, Result = "deny" };

        //获取用户token
        var tokens = _simpleRedis.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserToken, userId);
        if (tokens != null)
        {
            if (tokens.Any(it => it.Token == input.Password))//判断是否有token
                mqttAuthOutput.Result = "allow";//允许登录
        }
        return mqttAuthOutput;
    }


    #region 方法

    private async Task<List<DevConfig>> GetMqttConfig()
    {
        var key = RedisConst.Redis_DevConfig + CateGoryConst.Config_MQTT_BASE;//mqtt配置key
        //先从redis拿配置
        var configList = _simpleRedis.Get<List<DevConfig>>(key);
        if (configList == null)
        {
            //redis没有再去数据可拿
            configList = await DbContext.Db.Queryable<DevConfig>().Where(it => it.Category == CateGoryConst.Config_MQTT_BASE).ToListAsync();//获取mqtt配置配置列表
            if (configList.Count > 0)
            {
                _simpleRedis.Set(key, configList);//如果不为空,插入redis
            }
        }
        return configList;
    }
    #endregion
}
