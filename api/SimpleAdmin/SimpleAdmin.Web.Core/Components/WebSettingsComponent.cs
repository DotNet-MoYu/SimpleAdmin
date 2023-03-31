using SimpleAdmin.Plugin.Mqtt;
using SimpleAdmin.Plugin.SignalR;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// Web设置组件
/// 模拟 ConfigureService
/// </summary>
public sealed class WebSettingsComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        //web设置配置转实体
        services.AddConfigurableOptions<WebSettingsOptions>();
        //插件设置配置转实体
        services.AddConfigurableOptions<PluginSettingsOptions>();

        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var appSettings = App.GetConfig<WebSettingsOptions>("WebSettings", true);
        //如果是演示环境,加上操作筛选器,禁止操作数据库
        if (appSettings.EnvPoc)
            services.AddMvcFilter<MyActionFilter>();
        //插件设置
        StartPlugins(services);

    }

    private void StartPlugins(IServiceCollection services)
    {
        //获取插件配置
        var pluginSettings = App.GetConfig<PluginSettingsOptions>("PluginSettings");
        if (pluginSettings.UseSignalR)//如果使用signalr则启用signalr插件
            services.AddComponent<SignalRComponent>();
        if (pluginSettings.UseMqtt)//如果使用signalr则启用signalr插件
            services.AddComponent<MqttComponent>();
    }
}

/// <summary>
/// Web设置组件
/// 模拟 Configure
/// </summary>
public sealed class WebSettingsApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        //通过 App.GetOptions<TOptions> 获取选项
        var pluginSettings = App.GetOptions<PluginSettingsOptions>();
        if (pluginSettings.UseMqtt) app.UseComponent<MqttApplicationComponent>(env);

    }
}
