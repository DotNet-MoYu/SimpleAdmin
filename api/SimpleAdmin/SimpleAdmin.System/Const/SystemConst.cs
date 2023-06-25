namespace SimpleAdmin.System;

/// <summary>
/// 系统层常量
/// </summary>
public class SystemConst
{
    /// <summary>
    /// 系统配置表缓存Key
    /// </summary>
    public const string Cache_DevConfig = CacheConst.Cache_Prefix_Web + "DevConfig:";

    /// <summary>
    /// 登录验证码缓存Key
    /// </summary>
    public const string Cache_Captcha = CacheConst.Cache_Prefix_Web + "Captcha:";

    /// <summary>
    /// 用户表缓存Key
    /// </summary>
    public const string Cache_SysUser = CacheConst.Cache_Prefix_Web + "SysUser";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Cache_SysUserPhone = CacheConst.Cache_Prefix_Web + "SysUserPhone";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string Cache_SysUserAccount = CacheConst.Cache_Prefix_Web + "SysUserAccount";

    /// <summary>
    /// 资源表缓存Key
    /// </summary>
    public const string Cache_SysResource = CacheConst.Cache_Prefix_Web + "SysResource:";

    /// <summary>
    /// 字典表缓存Key
    /// </summary>
    public const string Cache_DevDict = CacheConst.Cache_Prefix_Web + "DevDict";

    /// <summary>
    /// 关系表缓存Key
    /// </summary>
    public const string Cache_SysRelation = CacheConst.Cache_Prefix_Web + "SysRelation:";

    /// <summary>
    /// 机构表缓存Key
    /// </summary>
    public const string Cache_SysOrg = CacheConst.Cache_Prefix_Web + "SysOrg";

    /// <summary>
    /// 角色表缓存Key
    /// </summary>
    public const string Cache_SysRole = CacheConst.Cache_Prefix_Web + "SysRole";

    /// <summary>
    /// 职位表缓存Key
    /// </summary>
    public const string Cache_SysPosition = CacheConst.Cache_Prefix_Web + "SysPosition";


    #region 登录错误次数

    /// <summary>
    ///  登录错误次数缓存Key
    /// </summary>
    public const string Cache_LoginErrorCount = CacheConst.Cache_Prefix_Web + "LoginErrorCount:";

    #endregion
}