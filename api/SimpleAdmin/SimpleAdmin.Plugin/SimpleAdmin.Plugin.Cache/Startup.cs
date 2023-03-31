namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //缓存设置配置转实体
        services.AddConfigurableOptions<CacheSettingsOptions>();
        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var cacheSettings = App.GetConfig<CacheSettingsOptions>("CacheSettings", true);
        //如果有redis连接字符串
        if (!string.IsNullOrEmpty(cacheSettings.ConnectionString))
        {
            //注入redis
            services.AddSimpleRedis(cacheSettings.ConnectionString);
            services.AddSingleton<ISimpleCacheService, RedisCacheService>();
        }
        else
        {
            services.AddSingleton<ISimpleCacheService, MemoryCacheService>();
        }





    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //通过 App.GetOptions<TOptions> 获取选项
        var cacheSettings = App.GetOptions<CacheSettingsOptions>();
        //如果需要清除缓存
        if (cacheSettings.ClearRedis && !string.IsNullOrEmpty(cacheSettings.ConnectionString))
        {
            var redis = App.GetService<ISimpleCacheService>();//获取redis服务
            //删除redis的key
            redis.DelByPattern(CacheConst.Cache_Prefix_Web);
        }


    }



}
