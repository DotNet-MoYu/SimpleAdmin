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
/// 资源
///</summary>
[SugarTable("sys_resource", TableDescription = "资源")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysResource : BaseEntity
{
    /// <summary>
    /// 父id
    ///</summary>
    [SugarColumn(ColumnName = "ParentId", ColumnDescription = "父id", IsNullable = true)]
    public virtual long? ParentId { get; set; }

    /// <summary>
    /// 标题
    ///</summary>
    [SugarColumn(ColumnName = "Title", ColumnDescription = "标题", Length = 200)]
    public virtual string Title { get; set; }

    /// <summary>
    /// 别名
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "别名", Length = 200, IsNullable = true)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    ///</summary>
    [SugarColumn(ColumnName = "Description", ColumnDescription = "描述", Length = 200, IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 编码
    ///</summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "编码", Length = 200, IsNullable = true)]
    public virtual string Code { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
    public string Category { get; set; }

    /// <summary>
    /// 模块
    ///</summary>
    [SugarColumn(ColumnName = "Module", ColumnDescription = "所属模块Id", IsNullable = true)]
    public virtual long? Module { get; set; }

    /// <summary>
    /// 菜单类型
    ///</summary>
    [SugarColumn(ColumnName = "MenuType", ColumnDescription = "菜单类型", Length = 200, IsNullable = true)]
    public virtual string MenuType { get; set; }

    /// <summary>
    /// 路径
    ///</summary>
    [SugarColumn(ColumnName = "Path", ColumnDescription = "路径", IsNullable = true)]
    public virtual string Path { get; set; }

    /// <summary>
    /// 组件
    ///</summary>
    [SugarColumn(ColumnName = "Component", ColumnDescription = "组件", Length = 200, IsNullable = true)]
    public string Component { get; set; }

    /// <summary>
    /// 图标
    ///</summary>
    [SugarColumn(ColumnName = "Icon", ColumnDescription = "图标", Length = 200, IsNullable = true)]
    public virtual string Icon { get; set; }

    /// <summary>
    /// 是否在菜单中隐藏, 需要高亮的 path (通常用作详情页高亮父级菜单)
    ///</summary>
    [SugarColumn(ColumnName = "ActiveMenu", ColumnDescription = "需要高亮的 path (通常用作详情页高亮父级菜单)", Length = 200, IsNullable = true)]
    public string ActiveMenu { get; set; }

    /// <summary>
    /// 是否在菜单中隐藏 (通常列表详情页需要隐藏)
    ///</summary>
    [SugarColumn(ColumnName = "IsHide", ColumnDescription = "是否在菜单中隐藏 (通常列表详情页需要隐藏) ", IsNullable = true)]
    public bool IsHide { get; set; }

    /// <summary>
    /// 是否在菜单中隐藏 (通常列表详情页需要隐藏)
    ///</summary>
    [SugarColumn(ColumnName = "IsFull", ColumnDescription = "菜单是否全屏 (示例：数据大屏页面)", IsNullable = true)]
    public bool IsFull { get; set; }

    /// <summary>
    /// 菜单是否固定在标签页中 (首页通常是固定项)
    ///</summary>
    [SugarColumn(ColumnName = "IsAffix", ColumnDescription = "菜单是否固定在标签页中 (首页通常是固定项)", IsNullable = true)]
    public bool IsAffix { get; set; }

    /// <summary>
    /// 菜单是否固定在标签页中 (首页通常是固定项)
    ///</summary>
    [SugarColumn(ColumnName = "IsKeepAlive", ColumnDescription = " 当前路由是否缓存", IsNullable = true)]
    public bool IsKeepAlive { get; set; }

    /// <summary>
    /// 是否为首页
    ///</summary>
    [SugarColumn(ColumnName = "IsHome", ColumnDescription = " 是否为首页", IsNullable = true)]
    public bool IsHome { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }

    /// <summary>
    /// 菜单元标签
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Meta Meta { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysResource> Children { get; set; }
}

/// <summary>
/// 菜单元标签
/// </summary>
public class Meta
{
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 是否在菜单中隐藏, 需要高亮的 path (通常用作详情页高亮父级菜单)
    ///</summary>
    public string ActiveMenu { get; set; }

    /// <summary>
    /// 是否在菜单中隐藏 (通常列表详情页需要隐藏)
    ///</summary>
    public bool IsHide { get; set; }

    /// <summary>
    /// 路由外链时填写的访问地址
    /// </summary>
    public string IsLink { get; set; } = "";

    /// <summary>
    /// 是否在菜单中隐藏 (通常列表详情页需要隐藏)
    ///</summary>
    public bool IsFull { get; set; }

    /// <summary>
    /// 菜单是否固定在标签页中 (首页通常是固定项)
    ///</summary>
    public bool IsAffix { get; set; }

    /// <summary>
    /// 菜单是否固定在标签页中 (首页通常是固定项)
    ///</summary>
    public bool IsKeepAlive { get; set; }
}
