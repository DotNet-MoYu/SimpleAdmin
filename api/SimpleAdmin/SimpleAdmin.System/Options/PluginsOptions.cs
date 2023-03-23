using Furion.ConfigurableOptions;

namespace SimpleAdmin.System;

/// <summary>
/// 插件配置选项
/// </summary>
public class PluginsOptions : IConfigurableOptions
{

    /// <summary>
    /// 是否开启SignalR 
    /// </summary>
    public bool UseSignalR { get; set; } = true;

    /// <summary>
    /// 是否开启Mqtt插件
    /// </summary>
    public bool UseMqtt { get; set; } = true;


    /// <summary>
    /// 默认通知类型
    /// SignalR/Mqtt
    /// </summary>
    public NoticeComponent NoticeComponent { get; set; } = NoticeComponent.Signalr;


}
