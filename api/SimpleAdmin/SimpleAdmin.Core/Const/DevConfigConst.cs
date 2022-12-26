using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 系统配置常量
/// </summary>
public class DevConfigConst
{
    /// <summary>
    /// 登录验证码开关
    /// </summary>
    public const string SYS_DEFAULT_CAPTCHA_OPEN = "SYS_DEFAULT_CAPTCHA_OPEN";

    /// <summary>
    /// 单用户登录开关
    /// </summary>
    public const string SYS_DEFAULT_SINGLE_OPEN = "SYS_DEFAULT_SINGLE_OPEN";

    /// <summary>
    /// 默认用户密码
    /// </summary>
    public const string SYS_DEFAULT_PASSWORD = "SYS_DEFAULT_PASSWORD";

    /// <summary>
    /// 系统默认工作台
    /// </summary>
    public const string SYS_DEFAULT_WORKBENCH_DATA_KEY = "SYS_DEFAULT_WORKBENCH_DATA_KEY";


    /// <summary>
    /// mqtt连接地址
    /// </summary>
    public const string MQTT_PARAM_URL = "MQTT_PARAM_URL";

    /// <summary>
    /// mqtt连接用户名
    /// </summary>
    public const string MQTT_PARAM_USERNAME = "MQTT_PARAM_USERNAME";

    /// <summary>
    /// mqtt连接密码
    /// </summary>
    public const string MQTT_PARAM_PASSWORD = "MQTT_PARAM_PASSWORD";




}
