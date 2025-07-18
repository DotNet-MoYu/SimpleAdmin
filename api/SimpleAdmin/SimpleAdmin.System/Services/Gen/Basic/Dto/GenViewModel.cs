using StringHelper = SimpleTool.StringHelper;

namespace SimpleAdmin.System;

/// <summary>
/// 代码生成绑定视图
/// </summary>
public class GenViewModel : GenBasic
{
    #region 基础

    /// <summary>
    /// 生成时间
    /// </summary>
    public string GenTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>
    /// 表字段
    /// </summary>
    public List<GenConfig> TableFields { get; set; }

    #endregion 基础

    #region 菜单

    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; } = CommonUtils.GetSingleId();

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string MenuCode { get; set; }

    /// <summary>
    /// /菜单路径
    /// </summary>
    public string MenuPath
    {
        get { return $"/{RouteName}/{BusName}"; }
    }

    /// <summary>
    /// /菜单路径
    /// </summary>
    public string MenuComponent
    {
        get { return $"{RouteName}/{BusName}/index"; }
    }

    #endregion 菜单

    #region 按钮

    /// <summary>
    /// 添加按钮ID
    /// </summary>
    public long AddButtonId { get; set; } = CommonUtils.GetSingleId();

    /// <summary>
    /// 批量删除按钮ID
    /// </summary>
    public long BatchDeleteButtonId { get; set; } = CommonUtils.GetSingleId();

    /// <summary>
    /// 编辑按钮ID
    /// </summary>
    public long EditButtonId { get; set; } = CommonUtils.GetSingleId();

    /// <summary>
    /// 删除按钮ID
    /// </summary>
    public long DeleteButtonId { get; set; } = CommonUtils.GetSingleId();

    #endregion 按钮


    #region 后端

    /// <summary>
    /// 类名首字母小写
    /// </summary>
    public string ClassNameFirstLower
    {
        get { return StringHelper.FirstCharToLower(ClassName); }
    }

    /// <summary>
    /// swagger分组名称
    /// </summary>
    public string ApiGroup
    {
        get
        {
            return ServicePosition.Split(".").Last();
        }
    }

    /// <summary>
    /// 服务名
    /// </summary>
    public string ServiceName
    {
        get { return ClassName + "Service"; }
    }

    /// <summary>
    /// 服务名开头首字母小写
    /// </summary>
    public string ServiceFirstLower
    {
        get { return ClassNameFirstLower + "Service"; }
    }

    #endregion 后端

    #region 注释描述

    /// <summary>
    /// 分页查询
    /// </summary>
    public string DescriptionPage
    {
        get { return FunctionName + "分页查询"; }
    }

    /// <summary>
    /// 列表查询
    /// </summary>
    public string DescriptionList
    {
        get { return FunctionName + "列表查询"; }
    }

    /// <summary>
    /// 添加
    /// </summary>
    public string DescriptionAdd
    {
        get { return "添加" + FunctionName; }
    }

    /// <summary>
    /// 修改
    /// </summary>
    public string DescriptionEdit
    {
        get { return "修改" + FunctionName; }
    }

    /// <summary>
    /// 修改
    /// </summary>
    public string DescriptionDelete
    {
        get { return "删除" + FunctionName; }
    }

    /// <summary>
    /// 详情
    /// </summary>
    public string DescriptionDetail
    {
        get { return FunctionName + "详情"; }
    }


    /// <summary>
    /// 导入
    /// </summary>
    public string DescriptionImport
    {
        get { return FunctionName + "导入"; }
    }

    /// <summary>
    /// 导入模板下载
    /// </summary>
    public string DescriptionTemplate
    {
        get { return FunctionName + "导入模板下载"; }
    }

    /// <summary>
    /// 导入预览
    /// </summary>
    public string DescriptionPreview
    {
        get { return FunctionName + "导入预览"; }
    }

    /// <summary>
    /// 导入
    /// </summary>
    public string DescriptionExport
    {
        get { return FunctionName + "导出"; }
    }

    /// <summary>
    /// 批量编辑
    /// </summary>
    public string DescriptionEdits
    {
        get { return FunctionName + "批量编辑"; }
    }

    /// <summary>
    /// 树查询
    /// </summary>
    public string DescriptionTree
    {
        get { return FunctionName + "树"; }
    }

    #endregion 注释描述

    #region 参数

    /// <summary>
    /// 分页参数
    /// </summary>
    public string PageInput
    {
        get { return ClassName + "PageInput"; }
    }

    /// <summary>
    /// 添加参数
    /// </summary>
    public string AddInput
    {
        get { return ClassName + "AddInput"; }
    }

    /// <summary>
    /// 编辑参数
    /// </summary>
    public string EditInput
    {
        get { return ClassName + "EditInput"; }
    }

    /// <summary>
    /// 树查询参数
    /// </summary>
    public string TreeInput
    {
        get { return ClassName + "TreeInput"; }
    }

    #endregion 参数
}
