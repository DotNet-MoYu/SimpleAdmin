namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    /// <summary>
    /// ConfigureServices中不能解析服务，比如App.GetService()，尤其是不能在ConfigureServices中获取诸如缓存等数据进行初始化，应该在Configure中进行
    /// 服务都还没初始化完成，会导致内存中存在多份 IOC 容器！！
    /// 正确应该在 Configure 中，这个时候服务（IServiceCollection 已经完成 BuildServiceProvider() 操作了
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        //缓存设置配置转实体
        services.AddConfigurableOptions<CacheSettingsOptions>();
        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var cacheSettings = App.GetConfig<CacheSettingsOptions>("CacheSettings", true);
        //如果有redis连接字符串
        if (cacheSettings.UseRedis)
        {
            var connectionString = $"server={cacheSettings.RedisSettings.Address};password={cacheSettings.RedisSettings.Password};db={cacheSettings.RedisSettings.Db}";
            //注入redis
            services.AddSimpleRedis(connectionString);
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
        if (cacheSettings.UseRedis && cacheSettings.RedisSettings.ClearRedis)
        {
            var redis = App.GetService<ISimpleCacheService>();//获取redis服务
            //删除redis的key
            redis.DelByPattern(CacheConst.Cache_Prefix_Web);
        }
    }
}