using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleAdmin.Plugin.Mqtt;
using SimpleAdmin.Plugin.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
///  系统组件
///  模拟 ConfigureService
/// </summary>
public sealed class SystemComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        //插件设置配置转实体
        services.AddConfigurableOptions<PluginsOptions>();
        //事件总线
        services.AddEventBus();
        //试图引擎
        services.AddViewEngine();
        //插件配置
        StartPlugins(services);

    }

    /// <summary>
    /// 启动插件
    /// </summary>
    /// <param name="services"></param>
    private void StartPlugins(IServiceCollection services)
    {
        var plugins = App.GetConfig<PluginsOptions>("Plugins");//获取配置
        if (plugins.UseSignalR)
            services.AddComponent<SignalRComponent>();//启动SignalR插件
        if (plugins.UseMqtt)
            services.AddComponent<MqttComponent>();//启动Mqtt插件
    }
}

/// <summary>
/// 系统设置组件
/// 模拟 Configure
/// </summary>
public sealed class SystemApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        // 获取插件选项
        var pluginsOptions = App.GetOptions<PluginsOptions>();
        //如果通知类型是mqtt
        if (pluginsOptions.NoticeComponent == NoticeComponent.Mqtt)
        {
            app.UseComponent<MqttApplicationComponent>(env);//加载mqtt组件
        }

    }
}
