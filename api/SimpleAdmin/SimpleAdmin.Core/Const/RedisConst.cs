using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// Redis常量
/// </summary>
public class RedisConst
{
    /// <summary>
    /// Redis Key前缀(可删除)
    /// </summary>
    public const string Redis_Prefix_Web = "SimpleAdminWeb:";

    /// <summary>
    /// Redis Key前缀(需要持久化，不随系统重启删除)
    /// </summary>
    public const string Redis_Prefix = "SimpleAdmin:";

    /// <summary>
    /// Redis Hash类型
    /// </summary>
    public const string Redis_Hash = "Hash";

    /// <summary>
    /// 系统配置表缓存Key
    /// </summary>
    public const string Redis_DevConfig = Redis_Prefix_Web + "DevConfig:";

    /// <summary>
    /// 登录验证码缓存Key
    /// </summary>
    public const string Redis_Captcha = Redis_Prefix_Web + "Captcha:";

    /// <summary>
    /// 用户表缓存Key
    /// </summary>
    public const string Redis_SysUser = Redis_Prefix_Web + "SysUser";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Redis_SysUserPhone = Redis_Prefix_Web + "SysUserPhone";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Redis_SysUserAccount = Redis_Prefix_Web + "SysUserAccount";

    /// <summary>
    /// 资源表缓存Key
    /// </summary>
    public const string Redis_SysResource = Redis_Prefix_Web + "SysResource:";

    /// <summary>
    /// 字典表缓存Key
    /// </summary>
    public const string Redis_DevDict = Redis_Prefix_Web + "DevDict";

    /// <summary>
    /// 关系表缓存Key
    /// </summary>
    public const string Redis_SysRelation = Redis_Prefix_Web + "SysRelation:";

    /// <summary>
    /// 机构表缓存Key
    /// </summary>
    public const string Redis_SysOrg = Redis_Prefix_Web + "SysOrg";

    /// <summary>
    /// 角色表缓存Key
    /// </summary>
    public const string Redis_SysRole = Redis_Prefix_Web + "SysRole";

    /// <summary>
    /// 职位表缓存Key
    /// </summary>
    public const string Redis_SysPosition = Redis_Prefix_Web + "SysPosition";

    /// <summary>
    /// mqtt认证登录信息key
    /// </summary>
    public const string Redis_MqttClientUser = Redis_Prefix_Web + "MqttClientUser:";

    /// <summary>
    /// 用户Token缓存Key
    /// </summary>
    public const string Redis_UserToken = Redis_Prefix + "UserToken";


}
