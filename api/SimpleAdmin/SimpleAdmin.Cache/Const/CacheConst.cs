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
    /// 系统配置表缓存Key
    /// </summary>
    public const string Cache_DevConfig = Cache_Prefix_Web + "DevConfig:";

    /// <summary>
    /// 登录验证码缓存Key
    /// </summary>
    public const string Cache_Captcha = Cache_Prefix_Web + "Captcha:";

    /// <summary>
    /// 用户表缓存Key
    /// </summary>
    public const string Cache_SysUser = Cache_Prefix_Web + "SysUser";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Cache_SysUserPhone = Cache_Prefix_Web + "SysUserPhone";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Cache_SysUserAccount = Cache_Prefix_Web + "SysUserAccount";

    /// <summary>
    /// 资源表缓存Key
    /// </summary>
    public const string Cache_SysResource = Cache_Prefix_Web + "SysResource:";

    /// <summary>
    /// 字典表缓存Key
    /// </summary>
    public const string Cache_DevDict = Cache_Prefix_Web + "DevDict";

    /// <summary>
    /// 关系表缓存Key
    /// </summary>
    public const string Cache_SysRelation = Cache_Prefix_Web + "SysRelation:";

    /// <summary>
    /// 机构表缓存Key
    /// </summary>
    public const string Cache_SysOrg = Cache_Prefix_Web + "SysOrg";

    /// <summary>
    /// 角色表缓存Key
    /// </summary>
    public const string Cache_SysRole = Cache_Prefix_Web + "SysRole";

    /// <summary>
    /// 职位表缓存Key
    /// </summary>
    public const string Cache_SysPosition = Cache_Prefix_Web + "SysPosition";

    /// <summary>
    /// mqtt认证登录信息key
    /// </summary>
    public const string Cache_MqttClientUser = Cache_Prefix_Web + "MqttClientUser:";

    /// <summary>
    /// 用户Token缓存Key
    /// </summary>
    public const string Cache_UserToken = Cache_Prefix + "UserToken";
}