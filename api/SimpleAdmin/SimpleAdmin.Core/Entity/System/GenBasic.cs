namespace SimpleAdmin.Core;

/// <summary>
/// 代码生成基础
///</summary>
[SugarTable("gen_basic", TableDescription = "代码生成基础")]
[Tenant(SqlsugarConst.DB_Default)]
public class GenBasic : BaseEntity
{

    /// <summary>
    /// 主表 
    ///</summary>
    [SugarColumn(ColumnName = "DbTable", ColumnDescription = "主表", Length = 200)]
    public virtual string DbTable { get; set; }

    /// <summary>
    /// 移除表前缀 
    ///</summary>
    [SugarColumn(ColumnName = "TablePrefix", ColumnDescription = "移除表前缀", Length = 200)]
    public virtual string TablePrefix { get; set; }
    /// <summary>
    /// 生成方式 
    ///</summary>
    [SugarColumn(ColumnName = "GenerateType", ColumnDescription = "生成方式", Length = 200)]
    public virtual string GenerateType { get; set; }
    /// <summary>
    /// 所属模块 
    ///</summary>
    [SugarColumn(ColumnName = "Module", ColumnDescription = "所属模块")]
    public virtual long Module { get; set; }
    /// <summary>
    /// 上级目录 
    ///</summary>
    [SugarColumn(ColumnName = "MenuPid", ColumnDescription = "上级目录")]
    public virtual long MenuPid { get; set; }

    /// <summary>
    /// 业务名 
    ///</summary>
    [SugarColumn(ColumnName = "RouteName", ColumnDescription = "路由名", Length = 200)]
    public virtual string RouteName { get; set; }

    /// <summary>
    /// 图标 
    ///</summary>
    [SugarColumn(ColumnName = "Icon", ColumnDescription = "图标", Length = 200)]
    public virtual string Icon { get; set; }
    /// <summary>
    /// 功能名 
    ///</summary>
    [SugarColumn(ColumnName = "FunctionName", ColumnDescription = "功能名", Length = 200)]
    public virtual string FunctionName { get; set; }
    /// <summary>
    /// 业务名 
    ///</summary>
    [SugarColumn(ColumnName = "BusName", ColumnDescription = "业务名", Length = 200)]
    public virtual string BusName { get; set; }
    /// <summary>
    /// 类名 
    ///</summary>
    [SugarColumn(ColumnName = "ClassName", ColumnDescription = "类名", Length = 200)]
    public virtual string ClassName { get; set; }
    /// <summary>
    /// 表单布局 
    ///</summary>
    [SugarColumn(ColumnName = "FormLayout", ColumnDescription = "表单布局", Length = 200)]
    public virtual string FormLayout { get; set; }
    /// <summary>
    /// 使用栅格 
    ///</summary>
    [SugarColumn(ColumnName = "GridWhether", ColumnDescription = "使用栅格", Length = 200)]
    public virtual string GridWhether { get; set; }


    /// <summary>
    /// 前端项目路径 
    ///</summary>
    [SugarColumn(ColumnName = "FrontedPath", ColumnDescription = "前端项目路径", IsNullable = true, Length = 200)]
    public virtual string FrontedPath { get; set; }
    /// <summary>
    /// 服务代码存放位置
    ///</summary>
    [SugarColumn(ColumnName = "ServicePosition", ColumnDescription = "服务代码存放位置", IsNullable = true, Length = 200)]
    public virtual string ServicePosition { get; set; }



    /// <summary>
    /// 控制器代码存放位置 
    ///</summary>
    [SugarColumn(ColumnName = "ControllerPosition", ColumnDescription = "控制器代码存放位置", IsNullable = true, Length = 200)]
    public virtual string ControllerPosition { get; set; }


    /// <summary>
    /// 作者 
    ///</summary>
    [SugarColumn(ColumnName = "AuthorName", ColumnDescription = "作者", Length = 200)]
    public virtual string AuthorName { get; set; }
    /// <summary>
    /// 排序 
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序")]
    public virtual int SortCode { get; set; }

}