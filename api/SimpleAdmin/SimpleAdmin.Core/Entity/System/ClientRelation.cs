namespace SimpleAdmin.Core.Entity.System;


/// <summary>
///  C端用户关系
/// </summary>
[SugarTable("client_relation", TableDescription = "关系")]
[Tenant(SqlsugarConst.DB_Default)]
public class ClientRelation : PrimaryKeyEntity
{
    /// <summary>
    /// 对象ID 
    ///</summary>
    [SugarColumn(ColumnName = "ObjectId", ColumnDescription = "对象ID", IsNullable = false)]
    public long ObjectId { get; set; }
    /// <summary>
    /// 目标ID 
    ///</summary>
    [SugarColumn(ColumnName = "TargetId", ColumnDescription = "目标ID", IsNullable = true)]
    public string TargetId { get; set; }
    /// <summary>
    /// 分类 
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200, IsNullable = true)]
    public string Category { get; set; }

}
