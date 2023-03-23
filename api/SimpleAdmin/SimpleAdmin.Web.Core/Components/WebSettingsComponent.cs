
using Microsoft.AspNetCore.Builder;
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
        //web设置配置转实体
        services.AddConfigurableOptions<WebSettingsOptions>();

        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var appSettings = App.GetConfig<WebSettingsOptions>("WebSettings", true);
        //如果是演示环境,加上操作筛选器,禁止操作数据库
        if (appSettings.EnvPoc)
            services.AddMvcFilter<MyActionFilter>();

        //启动业务层组件
        services.AddComponent<ApplicationComponent>();
        //启动System层组件
        services.AddComponent<SystemComponent>();

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
        //调用系统层ConfigureService
        app.UseComponent<SystemApplicationComponent>(env);
    }
}
