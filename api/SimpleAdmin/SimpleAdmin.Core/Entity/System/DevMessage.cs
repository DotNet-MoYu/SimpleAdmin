namespace SimpleAdmin.Core
{
    /// <summary>
    /// 站内信
    ///</summary>
    [SugarTable("dev_message", TableDescription = "站内信")]
    [Tenant(SqlsugarConst.DB_Default)]
    public class DevMessage : DataEntityBase
    {
        /// <summary>
        /// 分类 
        ///</summary>
        [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
        public virtual string Category { get; set; }
        /// <summary>
        /// 主题 
        ///</summary>
        [SugarColumn(ColumnName = "Subject", ColumnDescription = "主题")]
        public virtual string Subject { get; set; }
        /// <summary>
        /// 正文 
        ///</summary>
        [SugarColumn(ColumnName = "Content", ColumnDescription = "正文", ColumnDataType = SqlsugarConst.NVarCharMax, IsNullable = true)]
        public virtual string Content { get; set; }
    }
}
