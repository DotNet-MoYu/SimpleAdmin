namespace SimpleAdmin.System;

/// <summary>
/// 获取表字段输入
/// </summary>
public class GenBasicColumnInput
{
    /// <summary>
    /// 表名称
    /// </summary>
    [Required(ErrorMessage = "表名称不能为空")]
    public string TableName { get; set; }

}

/// <summary>
/// 代码生成基础添加参数
/// </summary>

public class GenBasicAddInput : GenBasic, IValidatableObject
{

    /// <summary>
    /// 主表名称
    /// </summary>
    [Required(ErrorMessage = "BbTable不能为空")]
    public override string DbTable { get; set; }



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
    [MinValue(1, ErrorMessage = "Module不能为空")]
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
        //如果是项目中生成
        if (GenerateType == GenConst.Pro)
        {
            if (string.IsNullOrEmpty(FrontedPath) || string.IsNullOrEmpty(ServicePosition) || string.IsNullOrEmpty(ControllerPosition))
            {
                yield return new ValidationResult("前端生成路径或后端代码存放位置不能为空", new[] { nameof(FrontedPath), nameof(ServicePosition), nameof(ControllerPosition) });
            }
        }

    }
}

/// <summary>
/// 代码生成基础编辑参数
/// </summary>
public class GenBasicEditInput : GenBasicAddInput
{
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}