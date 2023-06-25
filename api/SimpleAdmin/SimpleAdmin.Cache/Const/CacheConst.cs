namespace SimpleAdmin.Cache;

/// <summary>
/// Redis常量
/// </summary>
public class CacheConst
{
    /// <summary>
    /// Redis Key前缀(可删除)
    /// </summary>
    public const string Cache_Prefix_Web = "SimpleAdminWeb:";

    /// <summary>
    /// Redis Key前缀(需要持久化，不随系统重启删除)
    /// </summary>
    public const string Cache_Prefix = "SimpleAdmin:";

    /// <summary>
    /// Redis Hash类型
    /// </summary>
    public const string Cache_Hash = "Hash";


    /// <summary>
    /// 用户Token缓存Key
    /// </summary>
    public const string Cache_UserToken = Cache_Prefix + "UserToken";
}