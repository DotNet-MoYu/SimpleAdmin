using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;

namespace SimpleAdmin.Application;


/// <summary>
/// 学生信息
/// </summary>
[ExcelExporter(Name = "学生信息", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
public class GenTestExport
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
    /// 年龄
    /// </summary>
    [ExporterHeader(DisplayName = "年龄")]
    public int? Age { get; set; }

    [ExporterHeader(DisplayName = "民族")]
    public string Nation { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [ExporterHeader(DisplayName = "出生日期", Format = "yyyy-mm-DD")]
    public DateTime? Bir { get; set; }


}