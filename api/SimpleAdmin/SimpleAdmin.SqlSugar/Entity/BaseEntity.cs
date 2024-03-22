// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// 主键实体基类
/// </summary>
public abstract class PrimaryKeyEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true)]
    public virtual long Id { get; set; }

    /// <summary>
    /// 拓展信息
    /// </summary>
    [SugarColumn(ColumnName = "ExtJson", ColumnDescription = "扩展信息", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public virtual string ExtJson { get; set; }
}

/// <summary>
/// 框架实体基类
/// </summary>
public class BaseEntity : PrimaryKeyEntity
{
    [SugarColumn(ColumnName = "Status", ColumnDescription = "状态", Length = 20, DefaultValue = CommonStatusConst.ENABLE,
        IsNullable = true)]
    public virtual string Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true, IsNullable = true)]
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsOnlyIgnoreInsert = true, IsNullable = true)]
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者Id", IsOnlyIgnoreUpdate = true, IsNullable = true)]
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// 修改者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者Id", IsOnlyIgnoreInsert = true, IsNullable = true)]
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人", IsOnlyIgnoreUpdate = true, IsNullable = true)]
    public virtual string CreateUser { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [SugarColumn(ColumnDescription = "更新人", IsOnlyIgnoreInsert = true, IsNullable = true)]
    public virtual string UpdateUser { get; set; }

    /// <summary>
    /// 软删除
    /// </summary>
    [SugarColumn(ColumnDescription = "软删除", IsNullable = true)]
    public virtual bool IsDelete { get; set; }
}

/// <summary>
/// 业务数据实体基类(数据权限)
/// </summary>
public abstract class DataEntityBase : BaseEntity
{
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者部门Id", IsOnlyIgnoreUpdate = true, IsNullable = true)]
    public virtual long CreateOrgId { get; set; }
}
