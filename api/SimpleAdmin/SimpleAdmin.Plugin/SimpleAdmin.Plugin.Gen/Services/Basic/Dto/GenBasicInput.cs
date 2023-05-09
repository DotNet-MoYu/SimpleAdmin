namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// 代码生成基础添加参数
/// </summary>
public class GenBasicAddInput : GenBasic, IValidatableObject
{
    /// <summary>
    /// 所属库名称
    /// </summary>
    [Required(ErrorMessage = "ConfigId不能为空")]
    public override string ConfigId { get; set; }

    /// <summary>
    /// 主表名称
    /// </summary>
    [Required(ErrorMessage = "BbTable不能为空")]
    public override string DbTable { get; set; }

    /// <summary>
    ///  功能列表
    /// </summary>
    [Required(ErrorMessage = "FuncList不能为空")]
    public override List<string>? FuncList { get; set; }

    /// <summary>
    /// 生成模版
    /// </summary>
    [Required(ErrorMessage = "ModuleType不能为空")]
    public override string ModuleType { get; set; }

    /// <summary>
    ///  数据权限
    /// </summary>
    [Required(ErrorMessage = "DataPermission不能为空")]
    public override string DataPermission { get; set; }

    /// <summary>
    /// 实体名称
    /// </summary>
    [Required(ErrorMessage = "EntityName不能为空")]
    public override string EntityName { get; set; }

    /// <summary>
    /// 表前缀移除
    /// </summary>
    [Required(ErrorMessage = "TablePrefix不能为空")]
    public override string TablePrefix { get; set; }

    /// <summary>
    /// 生成方式
    /// </summary>
    [Required(ErrorMessage = "GenerateType不能为空")]
    public override string GenerateType { get; set; }

    /// <summary>
    /// 所属模块
    /// </summary>
    [IdNotNull(ErrorMessage = "Module不能为空")]
    public override long Module { get; set; }

    /// <summary>
    /// 路由名
    /// </summary>
    [Required(ErrorMessage = "RouteName不能为空")]
    public override string RouteName { get; set; }


    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Icon不能为空")]
    public override string Icon { get; set; }

    /// <summary>
    /// 功能名
    /// </summary>
    [Required(ErrorMessage = "FunctionName不能为空")]
    public override string FunctionName { get; set; }

    /// <summary>
    /// 业务名
    /// </summary>
    [Required(ErrorMessage = "BusName不能为空")]
    public override string BusName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    [Required(ErrorMessage = "ClassName不能为空")]
    public override string ClassName { get; set; }

    /// <summary>
    /// 表单布局
    /// </summary>
    [Required(ErrorMessage = "FormLayout不能为空")]
    public override string FormLayout { get; set; }

    /// <summary>
    /// 使用栅格
    /// </summary>
    [Required(ErrorMessage = "GridWhether不能为空")]
    public override string GridWhether { get; set; }

    /// <summary>
    /// 左侧树
    /// </summary>
    [Required(ErrorMessage = "LeftTree不能为空")]
    public override string LeftTree { get; set; }

    /// <summary>
    /// 前端路径
    /// </summary>
    [Required(ErrorMessage = "FrontedPath不能为空")]
    public override string FrontedPath { get; set; }

    /// <summary>
    /// 服务层
    /// </summary>
    [Required(ErrorMessage = "ServicePosition不能为空")]
    public override string ServicePosition { get; set; }

    /// <summary>
    /// 控制器层
    /// </summary>
    [Required(ErrorMessage = "ControllerPosition不能为空")]
    public override string ControllerPosition { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "SortCode不能为空")]
    public override int SortCode { get; set; }

    /// <summary>
    /// 作者名
    /// </summary>
    [Required(ErrorMessage = "AuthorName不能为空")]
    public override string AuthorName { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //如果是树形结构
        if (ModuleType.Contains("tree"))
        {
            if (string.IsNullOrEmpty(TreeId) || string.IsNullOrEmpty(TreeName) || string.IsNullOrEmpty(TreePid))
                yield return new ValidationResult($"必须配置树表相关信息", new[] { nameof(TreeId), nameof(TreeName), nameof(TreePid) });
        }
        //如果是树形结构
        if (ModuleType.Contains("master"))
        {
            if (string.IsNullOrEmpty(ChildFk) || string.IsNullOrEmpty(ChildTable))
                yield return new ValidationResult($"必须配置主子表相关信息", new[] { nameof(ChildTable), nameof(ChildFk) });
        }
        if (!FuncList.Contains("curd"))
        {
            yield return new ValidationResult($"必须包含基础的增删改查功能", new[] { nameof(FuncList) });
        }
    }
}

/// <summary>
/// 代码生成基础编辑参数
/// </summary>
public class GenBasicEditInput : GenBasicAddInput
{
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 执行代码生成参数
/// </summary>
public class ExecGenInput : BaseIdInput
{
    /// <summary>
    /// 生成类型
    /// </summary>
    public string ExecType { get; set; }
}