using System.Linq.Expressions;

namespace SimpleAdmin.System;

/// <summary>
/// 用户选择器参数
/// </summary>
public class UserSelectorInput
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
    public virtual string SearchKey { get; set; }


}

/// <summary>
/// 用户分页查询参数
/// </summary>
public class UserPageInput : BasePageInput
{
    /// <summary>
    /// 所属组织
    /// </summary>

    public long OrgId { get; set; }

    /// <summary>
    /// 动态查询条件
    /// </summary>
    public Expressionable<SysUser> Expression { get; set; }



    /// <summary>
    /// 用户状态
    /// </summary>

    public string UserStatus { get; set; }
}


/// <summary>
/// 添加用户参数
/// </summary>
public class UserAddInput : SysUser
{


    /// <summary>
    /// 账号
    /// </summary>
    [Required(ErrorMessage = "Account不能为空")]
    public override string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }

    /// <summary>
    /// 组织id
    /// </summary>
    [MinValue(1, ErrorMessage = "OrgId不能为空")]
    public override long OrgId { get; set; }

    /// <summary>
    /// 职位id
    /// </summary>
    [MinValue(1, ErrorMessage = "PositionId不能为空")]
    public override long PositionId { get; set; }
}

/// <summary>
/// 编辑用户参数
/// </summary>
public class UserEditInput : UserAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

}


/// <summary>
/// 用户授权角色参数
/// </summary>
public class UserGrantRoleInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    public long? Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "RoleIdList不能为空")]
    public List<long> RoleIdList { get; set; }
}
