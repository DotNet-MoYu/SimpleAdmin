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
/// 当前登录用户信息
/// </summary>
public class UserManager
{
    /// <summary>
    /// 当前用户Id
    /// </summary>
    public static long UserId
    {
        get
        {
            return (App.User?.FindFirst(ClaimConst.USER_ID)?.Value).ToLong();
        }
    }

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount
    {
        get
        {
            return App.User?.FindFirst(ClaimConst.ACCOUNT)?.Value;
        }
    }

    /// <summary>
    /// 当前用户昵称
    /// </summary>
    public static string Name
    {
        get
        {
            return App.User?.FindFirst(ClaimConst.NAME)?.Value;
        }
    }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool SuperAdmin
    {
        get
        {
            return (App.User?.FindFirst(ClaimConst.IS_SUPER_ADMIN)?.Value).ToBoolean();
        }
    }

    /// <summary>
    /// 机构ID
    /// </summary>
    public static long OrgId
    {
        get
        {
            return (App.User?.FindFirst(ClaimConst.ORG_ID)?.Value).ToLong();
        }
    }

    public static long? TenantId
    {
        get
        {
            //如果有租户ID则返回租户ID，否则返回null
            return (App.User?.FindFirst(ClaimConst.TENANT_ID)?.Value).ToLong();
        }
    }
}
