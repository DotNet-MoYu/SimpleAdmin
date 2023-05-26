namespace SimpleAdmin.SqlSugar;

/// <summary>
/// sqlsugar数据库配置
/// </summary>
public sealed class SqlSugarConfig : ConnectionConfig
{
    /// <summary>
    /// 是否驼峰转下划线
    /// </summary>
    public bool IsUnderLine { get; set; }
}