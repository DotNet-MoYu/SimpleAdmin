namespace SimpleAdmin.Core;

/// <summary>
/// Sqlsugar系统常量类
/// </summary>
public class SqlsugarConst
{
    /// <summary>
    /// 默认库ConfigId
    /// </summary>
    public const string DB_Default = "SimpleAdmin";


    /// <summary>
    /// 默认表主键
    /// </summary>
    public const string DB_PrimaryKey = "Id";

    #region 数据库字段类型
    /// <summary>
    /// varchar(max)
    /// </summary>
    public const string NVarCharMax = "nvarchar(max)";

    /// <summary>
    /// mysql的longtext
    /// </summary>
    public const string LongText = "longtext";

    /// <summary>
    /// sqlite的text
    /// </summary>
    public const string Text = "text";
    #endregion
}
