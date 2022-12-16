
using Shiny.Redis;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 系统设置组件
/// 模拟 ConfigureService
/// </summary>
public sealed class AppSettingsComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddConfigurableOptions<AppSettingsOptions>();

        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var appSettings = App.GetConfig<AppSettingsOptions>("AppSettings", true);
        //如果是演示环境,加上操作筛选器,禁止操作数据库
        if (appSettings.EnvPoc)
        {
            services.AddMvcFilter<MyActionFilter>();
        }


    }
}

/// <summary>
/// 系统设置组件
/// 模拟 Configure
/// </summary>
public sealed class AppSettingsApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        //通过 App.GetOptions<TOptions> 获取选项
        var appSettings = App.GetOptions<AppSettingsOptions>();
        //如果需要清除缓存
        if (appSettings.ClearRedis)
        {
            var redis = App.GetService<IRedisCacheManager>();//获取redis实例
            //删除redis的key
            redis.DelByPattern(RedisConst.Redis_Prefix_Web);
        }
    }
}
