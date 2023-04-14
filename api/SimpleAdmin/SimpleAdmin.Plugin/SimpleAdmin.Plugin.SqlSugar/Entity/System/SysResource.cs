namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 资源
///</summary>
[SugarTable("sys_resource", TableDescription = "资源")]
[Tenant(SqlsugarConst.DB_Default)]
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
    /// 颜色
    ///</summary>
    [SugarColumn(ColumnName = "Color", ColumnDescription = "颜色", Length = 200, IsNullable = true)]
    public string Color { get; set; }

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
    /// 字节点
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
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 是否首页
    /// </summary>
    public bool Affix { get; set; } = false;

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool hidden { get; set; } = false;
}