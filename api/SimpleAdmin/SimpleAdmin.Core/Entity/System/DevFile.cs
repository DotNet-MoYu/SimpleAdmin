namespace SimpleAdmin.Core;

/// <summary>
/// 文件表
///</summary>
[SugarTable("dev_file", TableDescription = "文件表")]
[Tenant(SqlsugarConst.DB_Default)]
public class DevFile : DataEntityBase
{
    /// <summary>
    /// 存储引擎 
    ///</summary>
    [SugarColumn(ColumnName = "Engine", ColumnDescription = "存储引擎", Length = 200)]
    public string Engine { get; set; }
    /// <summary>
    /// 存储桶 
    ///</summary>
    [SugarColumn(ColumnName = "Bucket", ColumnDescription = "存储桶", Length = 200)]
    public string Bucket { get; set; }
    /// <summary>
    /// 文件名称 
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "文件名称")]
    public string Name { get; set; }
    /// <summary>
    /// 文件后缀 
    ///</summary>
    [SugarColumn(ColumnName = "Suffix", ColumnDescription = "文件后缀", Length = 200)]
    public string Suffix { get; set; }
    /// <summary>
    /// 文件大小kb 
    ///</summary>
    [SugarColumn(ColumnName = "SizeKb", ColumnDescription = "文件大小kb")]
    public long SizeKb { get; set; }
    /// <summary>
    /// 文件大小（格式化后） 
    ///</summary>
    [SugarColumn(ColumnName = "SizeInfo", ColumnDescription = "文件大小（格式化后）", Length = 200)]
    public string SizeInfo { get; set; }
    /// <summary>
    /// 文件的对象名（唯一名称） 
    ///</summary>
    [SugarColumn(ColumnName = "ObjName", ColumnDescription = "文件的对象名（唯一名称）")]
    public string ObjName { get; set; }
    /// <summary>
    /// 文件存储路径 
    ///</summary>
    [SugarColumn(ColumnName = "StoragePath", ColumnDescription = "文件存储路径")]
    public string StoragePath { get; set; }
    /// <summary>
    /// 文件下载路径 
    ///</summary>
    [SugarColumn(ColumnName = "DownloadPath", ColumnDescription = "文件下载路径", IsNullable = true)]
    public string DownloadPath { get; set; }
    /// <summary>
    /// 图片缩略图 
    ///</summary>
    [SugarColumn(ColumnName = "Thumbnail", ColumnDescription = "图片缩略图", IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string Thumbnail { get; set; }

}
