using Furion.ConfigurableOptions;

namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// 缓存设置
/// </summary>
public class CacheSettingsOptions : IConfigurableOptions
{
    /// <summary>
    /// 使用Redis
    /// </summary>
    public bool UserRedis { get; set; }

    /// <summary>
    /// 是否每次启动都清空
    /// </summary>
    public RedisSettings RedisSettings { get; set; }
}

/// <summary>
/// Redis设置
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// 连接地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 数据库
    /// </summary>
    public int Db { get; set; } = 0;

    /// <summary>
    /// 是否每次启动都清空
    /// </summary>
    public bool ClearRedis { get; set; } = false;
}