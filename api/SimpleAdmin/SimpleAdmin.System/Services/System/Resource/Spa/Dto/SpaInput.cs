namespace SimpleAdmin.System;

/// <summary>
/// 单页输入参数
/// </summary>
public class SpaPageInput : BasePageInput
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public string MenuType { get; set; }
}

/// <summary>
/// 单页输入参数
/// </summary>
public class SpaAddInput : SysResource
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "Title不能为空")]
    public override string Title { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    [Required(ErrorMessage = "Title不能为空")]
    public override string MenuType { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Required(ErrorMessage = "Path不能为空")]
    public override string Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Icon不能为空")]
    public override string Icon { get; set; }
}

/// <summary>
/// 单页修改参数
/// </summary>
public class SpaEditInput : SpaAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}