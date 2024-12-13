﻿// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core;

public class MqttConst
{
    /// <summary>
    /// mqtt认证登录信息key
    /// </summary>
    public const string CACHE_MQTT_CLIENT_USER = CacheConst.CACHE_PREFIX_WEB + "MqttClientUser:";

    /// <summary>
    /// mqtt主题前缀
    /// </summary>
    public const string MQTT_TOPIC_PREFIX = "SimpleAdmin/";


    /// <summary>
    /// 登出
    /// </summary>
    public const string MQTT_MESSAGE_LOGIN_OUT = "LoginOut";

    /// <summary>
    /// 新消息
    /// </summary>
    public const string MQTT_MESSAGE_NEW = "NewMessage";

    /// <summary>
    /// 修改密码
    /// </summary>
    public const string MQTT_MESSAGE_UPDATE_PASSWORD = "UpdatePassword";
}
