// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
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
    public const string CACHE_DEV_CONFIG = CacheConst.CACHE_PREFIX_WEB + "SysConfig:";

    /// <summary>
    /// 登录验证码缓存Key
    /// </summary>
    public const string CACHE_CAPTCHA = CacheConst.CACHE_PREFIX_WEB + "Captcha:";

    /// <summary>
    /// 用户表缓存Key
    /// </summary>
    public const string CACHE_SYS_USER = CacheConst.CACHE_PREFIX_WEB + "SysUser";

    /// <summary>
    /// 用户头像缓存关系Key
    /// </summary>
    public const string CACHE_SYS_USER_AVATAR = CacheConst.CACHE_PREFIX_WEB + "SysUserAvatar";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string CACHE_SYS_USER_PHONE = CacheConst.CACHE_PREFIX_WEB + "SysUserPhone";

    /// <summary>
    /// 用户手机号关系缓存Key
    /// </summary>
    public const string CACHE_SYS_USER_ACCOUNT = CacheConst.CACHE_PREFIX_WEB + "SysUserAccount";

    /// <summary>
    /// 资源表缓存Key
    /// </summary>
    public const string CACHE_SYS_RESOURCE = CacheConst.CACHE_PREFIX_WEB + "SysResource:";

    /// <summary>
    /// 字典表缓存Key
    /// </summary>
    public const string CACHE_DEV_DICT = CacheConst.CACHE_PREFIX_WEB + "SysDict";

    /// <summary>
    /// 关系表缓存Key
    /// </summary>
    public const string CACHE_SYS_RELATION = CacheConst.CACHE_PREFIX_WEB + "SysRelation:";

    /// <summary>
    /// 机构表缓存Key
    /// </summary>
    public const string CACHE_SYS_ORG = CacheConst.CACHE_PREFIX_WEB + "SysOrg";

    /// <summary>
    /// 机构对应租户关系缓存Key
    /// </summary>
    public const string CACHE_SYS_ORG_TENANT = CacheConst.CACHE_PREFIX_WEB + "SysOrgTenant";

    /// <summary>
    /// 租户列表缓存
    /// </summary>
    public const string CACHE_SYS_TENANT = CacheConst.CACHE_PREFIX_WEB + "Tenant";

    /// <summary>
    /// 角色表缓存Key
    /// </summary>
    public const string CACHE_SYS_ROLE = CacheConst.CACHE_PREFIX_WEB + "SysRole";

    /// <summary>
    /// 职位表缓存Key
    /// </summary>
    public const string CACHE_SYS_POSITION = CacheConst.CACHE_PREFIX_WEB + "SysPosition";

    #region 登录错误次数

    /// <summary>
    ///  登录错误次数缓存Key
    /// </summary>
    public const string CACHE_LOGIN_ERROR_COUNT = CacheConst.CACHE_PREFIX_WEB + "LoginErrorCount:";

    #endregion 登录错误次数

    #region 操作

    /// <summary>
    /// 添加操作
    /// </summary>
    public const string ADD = "添加";

    /// <summary>
    /// 编辑操作
    /// </summary>
    public const string EDIT = "编辑";

    /// <summary>
    /// 启用操作
    /// </summary>
    public const string ENABLE = "启用";

    /// <summary>
    /// 禁用操作
    /// </summary>
    public const string DISABLE = "禁用";

    /// <summary>
    /// 重置密码操作
    /// </summary>
    public const string RESET_PWD = "重置密码";

    /// <summary>
    /// 用户授权操作
    /// </summary>
    public const string GRANT_ROLE = "授权";

    #endregion 操作

    #region 别称

    /// <summary>
    /// 组织
    /// </summary>
    public const string SYS_ORG = "组织";


    /// <summary>
    /// 职位
    /// </summary>
    public const string SYS_POS = "职位";

    #endregion 别称
}
