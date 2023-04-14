namespace SimpleAdmin.Plugin.ImportExport;

/// <summary>
/// 导入基础输入
/// </summary>
[ExcelImporter(IsLabelingError = true)]
public class ImportTemplateInput
{
    /// <summary>
    /// Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    public long Id { get; set; } = CommonUtils.GetSingleId();

    /// <summary>
    /// 是否有错误
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    public bool HasError { get; set; } = false;

    /// <summary>
    /// 错误详情
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    public IDictionary<string, string> ErrorInfo { get; set; } = new Dictionary<string, string>();
}