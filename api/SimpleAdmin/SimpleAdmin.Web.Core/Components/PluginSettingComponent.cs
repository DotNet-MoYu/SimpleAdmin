// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using SimpleAdmin.Plugin.Mqtt;
using SimpleAdmin.Plugin.SignalR;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 插件设置组件
/// 模拟 ConfigureService
/// </summary>
public sealed class PluginSettingComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        //插件设置配置转实体
        services.AddConfigurableOptions<PluginSettingsOptions>();
        //获取插件配置
        var pluginSettings = App.GetConfig<PluginSettingsOptions>("PluginSettings");
        if (pluginSettings.UseSignalR)//如果使用signalr则启用signalr插件
            services.AddComponent<SignalRComponent>();
        if (pluginSettings.UseMqtt)//如果使用mqtt则启用mqtt插件
            services.AddComponent<MqttComponent>();
    }
}

/// <summary>
/// 插件设置组件
/// 模拟 Configure
/// </summary>
public sealed class PluginSettingsApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        //通过 App.GetOptions<TOptions> 获取选项
        var pluginSettings = App.GetOptions<PluginSettingsOptions>();
        if (pluginSettings.UseMqtt) app.UseComponent<MqttApplicationComponent>(env);
    }
}