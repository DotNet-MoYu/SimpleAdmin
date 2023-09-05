// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 系统层常量
/// </summary>
public class SystemConst
{
    /// <summary>
    /// 系统配置表缓存Key
    /// </summary>
    public const string Cache_DevConfig = CacheConst.Cache_Prefix_Web + "SysConfig:";

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
    public const string Cache_DevDict = CacheConst.Cache_Prefix_Web + "SysDict";

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

    #endregion 登录错误次数
}