using Magicodes.ExporterAndImporter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;


/// <summary>
/// 文件导入通用输出
/// </summary>
public class ImportPreviewOutput<T> where T : class
{
    /// <summary>
    /// 是否有错误
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// 动态表头
    /// </summary>
    public List<TableColumns> TableColumns { get; set; } = new List<TableColumns>();


    /// <summary>
    /// 数据
    /// </summary>
    public List<T> Data { get; set; }

}

/// <summary>
/// 动态表头
/// </summary>
public class TableColumns
{

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }


    /// <summary>
    /// 数据源
    /// </summary>
    public string DataIndex { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; } = 100;


    /// <summary>
    /// 超过宽度将自动省略
    /// </summary>
    public bool Ellipsis { get; set; } = false;


    /// <summary>
    /// 是否是日期格式
    /// </summary>
    public bool Date { get; set; } = false;

}
