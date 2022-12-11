namespace SimpleAdmin.System;

/// <summary>
/// 模块分页输入
/// </summary>
public class ModulePageInput : BasePageInput
{

}

/// <summary>
/// 添加模块输入
/// </summary>
public class ModuleAddInput : SysResource
{

}

/// <summary>
/// 编辑模块输入
/// </summary>
public class ModuleEditInput : ModuleAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

}



