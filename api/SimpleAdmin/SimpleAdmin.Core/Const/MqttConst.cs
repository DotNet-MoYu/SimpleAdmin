using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// mqtt常量
/// </summary>
public class MqttConst
{

    /// <summary>
    /// mqtt主题前缀
    /// </summary>
    public const string Mqtt_TopicPrefix = "SimpleAdmin/";

    /// <summary>
    /// 登出
    /// </summary>
    public const string Mqtt_Message_LoginOut = "LoginOut";
}
