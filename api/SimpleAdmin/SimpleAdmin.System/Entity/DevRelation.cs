namespace SimpleAdmin.System;

/// <summary>
/// 开发关系表
///</summary>
[SugarTable("dev_relation", TableDescription = "开发关系表")]
[Tenant(SqlsugarConst.DB_Default)]
public class DevRelation : SysRelation
{
}