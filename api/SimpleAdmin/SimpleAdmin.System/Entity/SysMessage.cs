// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 站内信
///</summary>
[SugarTable("sys_message", TableDescription = "站内信")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysMessage : BaseEntity
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
    /// 接收人类型
    /// </summary>
    [SugarColumn(ColumnName = "ReceiverType", ColumnDescription = "接收人类型")]
    public virtual string ReceiverType { get; set; }

    /// <summary>
    /// 接收人列表
    /// </summary>
    [SugarColumn(ColumnName = "ReceiverInfo", ColumnDescription = "接收人列表", IsJson = true)]
    public virtual List<ReceiverInfo> ReceiverInfo { get; set; }

    /// <summary>
    /// 发送方式
    /// </summary>
    [SugarColumn(ColumnName = "SendWay", ColumnDescription = "发送方式")]
    public virtual string SendWay { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [SugarColumn(ColumnDescription = "发送时间")]
    public virtual DateTime SendTime { get; set; }

    /// <summary>
    /// 正文
    ///</summary>
    [SugarColumn(ColumnName = "Content", ColumnDescription = "正文", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public virtual string Content { get; set; }


    /// <summary>
    /// 延迟时间(秒)
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int DelayTime { get; set; }

    /// <summary>
    /// 已读情况
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<ReceiverDetail> ReceiverDetail { get; set; } = new List<ReceiverDetail>();

    /// <summary>
    /// 发送时间格式化
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string SendTimeFormat { get; set; }

    /// <summary>
    /// 分组查询用
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int Index2 { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public bool Read { get; set; }
}

public class ReceiverInfo
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
}

public class ReceiverDetail
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool Read { get; set; }
}
