using SimpleAdmin.Plugin.Mqtt;
using SimpleAdmin.Plugin.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

