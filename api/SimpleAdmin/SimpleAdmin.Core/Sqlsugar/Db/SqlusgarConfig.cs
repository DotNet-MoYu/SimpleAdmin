namespace SimpleAdmin.Core;

/// <summary>
/// sqlsugar数据库配置
/// </summary>
public sealed class SqlSugarConfig : ConnectionConfig
{
    /// <summary>
    /// 是否初始化数据库
    /// </summary>
    public bool IsInitDb { get; set; }

    /// <summary>
    /// 是否初始化种子数据
    /// </summary>
    public bool IsSeedData { get; set; }

}
