namespace SimpleAdmin.System;

/// <summary>
/// 角色拥有的资源输出
/// </summary>
public class RoleOwnResourceOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// 已授权资源信息
    /// </summary>
    public virtual List<RelationRoleResuorce> GrantInfoList { get; set; }


}

/// <summary>
/// 角色拥有权限输出
/// </summary>
public class RoleOwnPermissionOutput
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// 已授权资源信息
    /// </summary>
    public virtual List<RelationRolePermission> GrantInfoList { get; set; }


}
