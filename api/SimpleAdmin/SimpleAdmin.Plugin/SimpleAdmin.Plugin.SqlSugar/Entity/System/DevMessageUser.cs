namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 用户消息表
///</summary>
[SugarTable("dev_message_user", TableDescription = "用户消息表")]
[Tenant(SqlsugarConst.DB_Default)]
public class DevMessageUser : BaseEntity
{
    /// <summary>
    /// 消息Id
    ///</summary>
    [SugarColumn(ColumnName = "MessageId", ColumnDescription = "消息Id", IsNullable = false)]
    public long MessageId { get; set; }

    /// <summary>
    /// 用户Id
    ///</summary>
    [SugarColumn(ColumnName = "UserId", ColumnDescription = "用户Id", IsNullable = false)]
    public long UserId { get; set; }

    /// <summary>
    /// 已读未读
    ///</summary>
    [SugarColumn(ColumnName = "Read", ColumnDescription = "已读未读", IsNullable = false)]
    public bool Read { get; set; }
}