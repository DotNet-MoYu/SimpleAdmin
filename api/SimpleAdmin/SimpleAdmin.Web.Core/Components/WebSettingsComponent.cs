
using Microsoft.AspNetCore.SignalR;
using SimpleMQTT;
using SimpleRedis;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// Web设置组件
/// 模拟 ConfigureService
/// </summary>
public sealed class WebSettingsComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddConfigurableOptions<WebSettingsOptions>();

        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var appSettings = App.GetConfig<WebSettingsOptions>("WebSettings", true);
        //如果是演示环境,加上操作筛选器,禁止操作数据库
        if (appSettings.EnvPoc)
            services.AddMvcFilter<MyActionFilter>();
        //如果使用mqtt，注册mqtt服务
        if (appSettings.UseMqtt)
            services.AddMqttClientManager();
        else
        {
            services.AddSignalR();//注册SignalR
            services.AddSingleton<IUserIdProvider, UserIdProvider>();//用户ID提供器
        }

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
        var webSettings = App.GetOptions<WebSettingsOptions>();
        //如果需要清除缓存
        if (webSettings.ClearRedis)
        {
            var redis = App.GetService<ISimpleRedis>();//获取redis服务
            //删除redis的key
            redis.DelByPattern(RedisConst.Redis_Prefix_Web);
        }
        if (webSettings.UseMqtt)
        {
            var mqttClientManager = App.GetService<IMqttClientManager>();//获取mqtt服务判断配置是否有问题
        }

    }
}
