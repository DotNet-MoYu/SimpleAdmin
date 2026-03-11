// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

/// <summary>
/// 文件节点类型
/// </summary>
public enum DocumentNodeType
{
    /// <summary>
    /// 文件夹
    /// </summary>
    Folder = 1,

    /// <summary>
    /// 文件
    /// </summary>
    File = 2
}

/// <summary>
/// 文件节点
/// </summary>
[SugarTable("biz_document", TableDescription = "业务文件")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
[CodeGen]
public class BizDocument : DataEntityBase
{
    /// <summary>
    /// 父级ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 授权根目录ID
    /// </summary>
    public long RootId { get; set; }

    /// <summary>
    /// 祖先链
    /// </summary>
    [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string Ancestors { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(Length = 255)]
    public string Name { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    public DocumentNodeType NodeType { get; set; }

    /// <summary>
    /// 文件ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public long? FileId { get; set; }

    /// <summary>
    /// 存储引擎
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string Engine { get; set; }

    /// <summary>
    /// 文件大小KB
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public long? SizeKb { get; set; }

    /// <summary>
    /// 后缀名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string Suffix { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string Label { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 子节点
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<BizDocument> Children { get; set; } = new();
}
