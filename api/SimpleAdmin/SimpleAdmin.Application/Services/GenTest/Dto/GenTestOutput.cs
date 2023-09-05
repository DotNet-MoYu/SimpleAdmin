// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

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