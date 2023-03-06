namespace SimpleAdmin.System;

/// <summary>
/// 职位分页查询
/// </summary>
public class PositionPageInput : BasePageInput
{

    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 职位列表
    /// </summary>
    public List<long> OrgIds { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; set; }
}

/// <summary>
/// 职位新增参数
/// </summary>
public class PositionAddInput : SysPosition
{
    /// <summary>
    /// 组织ID
    /// </summary>
    [IdNotNull(ErrorMessage = "OrgId不能为空")]
    public override long OrgId { get; set; }

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
/// 机构编辑参数
/// </summary>
public class PositionEditInput : PositionAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 机构选择器参数
/// </summary>
public class PositionSelectorInput : UserSelectorInput
{

}
