namespace SimpleAdmin.System;


/// <summary>
/// 组织分页查询参数
/// </summary>
public class OrgPageInput : BasePageInput
{

    /// <summary>
    /// 父ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 机构列表
    /// </summary>
    public List<long> OrgIds { get; set; }

}

/// <summary>
/// 组织添加参数
/// </summary>
public class OrgAddInput : SysOrg
{

}

/// <summary>
/// 组织修改参数
/// </summary>
public class OrgEditInput : OrgAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}
