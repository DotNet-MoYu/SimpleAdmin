namespace SimpleAdmin.System;

/// <summary>
/// 当前登录用户信息
/// </summary>
public class UserManager
{


    /// <summary>
    /// 当前用户Id
    /// </summary>
    public static long UserId => (App.User?.FindFirst(ClaimConst.UserId)?.Value).ToLong();

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount => App.User?.FindFirst(ClaimConst.Account)?.Value;

    /// <summary>
    /// 当前用户昵称
    /// </summary>
    public static string Name => App.User?.FindFirst(ClaimConst.Name)?.Value;

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool SuperAdmin => (App.User?.FindFirst(ClaimConst.IsSuperAdmin)?.Value).ToBoolean();


    /// <summary>
    /// 机构ID
    /// </summary>
    public static long OrgId => (App.User?.FindFirst(ClaimConst.OrgId)?.Value).ToLong();
}
