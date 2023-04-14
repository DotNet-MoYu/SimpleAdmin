namespace SimpleAdmin.Plugin.Aop;

/// <summary>
/// 缓存结果特性
/// </summary>
public class CacheAttribute : Attribute
{
    /// <summary>
    /// RedisKey前缀
    /// </summary>
    public string KeyPrefix { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan? AbsoluteExpiration { get; set; }

    /// <summary>
    /// 自定义KEY
    /// </summary>
    public string CustomKeyValue { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDelete { get; set; } = false;

    /// <summary>
    /// 存储类型
    /// </summary>
    public string StoreType { get; set; }
}