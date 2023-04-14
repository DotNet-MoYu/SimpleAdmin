namespace SimpleAdmin.Plugin.ImportExport;

/// <summary>
/// 前端表格特性
/// </summary>
public class AntTableAttribute : Attribute
{
    /// <summary>
    /// 是否是日期格式
    /// </summary>
    public bool IsDate { get; set; } = false;

    /// <summary>
    /// 是否自动省略
    /// </summary>
    public bool Ellipsis { get; set; } = false;

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; } = 100;
}