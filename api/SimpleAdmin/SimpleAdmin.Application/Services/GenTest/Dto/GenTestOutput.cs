namespace SimpleAdmin.Application;

/// <summary>
/// 测试导出
/// </summary>
[ExcelExporter(Name = "测试信息", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
public class GenTestExportOutput
{
  /// <summary>
  /// 姓名
  /// </summary>
  [ExporterHeader(DisplayName = "姓名")]
  public string Name { get; set; }
  
  /// <summary>
  /// 性别
  /// </summary>
  [ExporterHeader(DisplayName = "性别")]
  public string Sex { get; set; }
  
  /// <summary>
  /// 民族
  /// </summary>
  [ExporterHeader(DisplayName = "民族")]
  public string Nation { get; set; }
  
  /// <summary>
  /// 年龄
  /// </summary>
  [ExporterHeader(DisplayName = "年龄")]
  public string Age { get; set; }
  
  /// <summary>
  /// 生日
  /// </summary>
  [ExporterHeader(DisplayName = "生日")]
  public string Bir { get; set; }
  
  /// <summary>
  /// 排序码
  /// </summary>
  [ExporterHeader(DisplayName = "排序码")]
  public string SortCode { get; set; }
  
  /// <summary>
  /// 存款
  /// </summary>
  [ExporterHeader(DisplayName = "存款")]
  public string Money { get; set; }
  
}



