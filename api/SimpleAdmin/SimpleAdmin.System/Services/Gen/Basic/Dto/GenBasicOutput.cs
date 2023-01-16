namespace SimpleAdmin.System;

/// <summary>
/// 表信息输出
/// </summary>

public class GenBasicTableOutput
{

    /// <summary>
    /// 表名称
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 实体名
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// 表注释
    /// </summary>
    public string TableRemark { get; set; }


}


/// <summary>
/// 表字段信息输出
/// </summary>
public class GenBasicColumnOutput
{

    /// <summary>
    /// 字段名称
    /// </summary>
    public string ColumnName { get; set; }


    /// <summary>
    /// 是否主键
    /// </summary>
    public bool IsPrimarykey { get; set; }

    /// <summary>
    /// 字段类型
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 字段注释
    /// </summary>
    public string ColumnRemark { get; set; }

}

/// <summary>
/// 预览代码生成结果
/// </summary>
public class GenBasePreviewOutput
{
    /// <summary>
    /// SQL代码结果集
    /// </summary>
    public List<GenBaseCodeResult> SqlResults { get; set; } = new List<GenBaseCodeResult>();

    /// <summary>
    /// 前端代码结果集
    /// </summary>
    public List<GenBaseCodeResult> CodeFrontendResults { get; set; } = new List<GenBaseCodeResult>();

    /// <summary>
    /// 后端代码结果集
    /// </summary>

    public List<GenBaseCodeResult> CodeBackendResults { get; set; } = new List<GenBaseCodeResult>();


    public class GenBaseCodeResult
    {
        /// <summary>
        /// 代码文件名称
        /// </summary>
        public string CodeFileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 代码文件内容
        /// </summary>
        public string CodeFileContent { get; set; }
    }
}