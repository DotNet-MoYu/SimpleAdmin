namespace SimpleAdmin.System;


/// <summary>
/// 角色查询参数
/// </summary>
public class RolePageInput : PositionPageInput
{

}

/// <summary>
/// 角色添加参数
/// </summary>
public class RoleAddInput : SysRole
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    [Required(ErrorMessage = "Category不能为空")]
    public override string Category { get; set; }
}

/// <summary>
/// 角色编辑参数
/// </summary>
public class RoleEditInput : RoleAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 角色授权资源参数
/// </summary>
public class GrantResourceInput : RoleOwnResourceOutput
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    /// <summary>
    /// 授权资源信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public override List<RelationRoleResuorce> GrantInfoList { get; set; }

    /// <summary>
    /// 是否代码生成
    /// </summary>
    public bool IsCodeGen { get; set; }
}

/// <summary>
/// 角色授权资源参数
/// </summary>
public class GrantPermissionInput : RoleOwnPermissionOutput
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public override List<RelationRolePermission> GrantInfoList { get; set; }
}

/// <summary>
/// 角色授权用户参数
/// </summary>
public class GrantUserInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public List<long> GrantInfoList { get; set; }
}

/// <summary>
/// 角色选择器参数
/// </summary>
public class RoleSelectorInput
{
    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }



    /// <summary>
    /// 机构ID列表
    /// </summary>
    public List<long> OrgIds { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string SearchKey { get; set; }

    /// <summary>
    /// 角色分类
    /// </summary>
    public string Category { get; set; }
}

