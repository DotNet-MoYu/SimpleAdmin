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
    /// Redis Key前缀
    /// </summary>
    public const string Redis_Prefix = "SimpleAdmin:";

    /// <summary>
    /// Redis Hash类型
    /// </summary>
    public const string Redis_Hash = "Hash";

    /// <summary>
    /// 系统配置表缓存Key
    /// </summary>
    public const string Redis_DevConfig = Redis_Prefix + "DevConfig:";

    /// <summary>
    /// 登录验证码缓存Key
    /// </summary>
    public const string Redis_Captcha = Redis_Prefix + "Captcha:";

    /// <summary>
    /// 用户表缓存Key
    /// </summary>
    public const string Redis_SysUser = Redis_Prefix + "SysUser";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Redis_SysUserPhone = Redis_Prefix + "SysUserPhone";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Redis_SysUserAccount = Redis_Prefix + "SysUserAccount";

    /// <summary>
    /// 资源表缓存Key
    /// </summary>
    public const string Redis_SysResource = Redis_Prefix + "SysResource:";

    /// <summary>
    /// 字典表缓存Key
    /// </summary>
    public const string Redis_DevDict = Redis_Prefix + "DevDict";

    /// <summary>
    /// 关系表缓存Key
    /// </summary>
    public const string Redis_SysRelation = Redis_Prefix + "SysRelation:";

    /// <summary>
    /// 机构表缓存Key
    /// </summary>
    public const string Redis_SysOrg = Redis_Prefix + "SysOrg";

    /// <summary>
    /// 角色表缓存Key
    /// </summary>
    public const string Redis_SysRole = Redis_Prefix + "SysRole";

    /// <summary>
    /// 职位表缓存Key
    /// </summary>
    public const string Redis_SysPosition = Redis_Prefix + "SysPosition";

}
