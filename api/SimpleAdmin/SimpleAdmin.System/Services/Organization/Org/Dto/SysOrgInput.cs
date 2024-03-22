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
/// 组织分页查询参数
/// </summary>
public class SysOrgPageInput : BasePageInput
{
    /// <summary>
    /// 父ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    ///  状态
    /// </summary>
    public string Status { get; set; }
}

/// <summary>
/// 组织添加参数
/// </summary>
public class SysOrgAddInput : SysOrg
{
}

/// <summary>
/// 组织修改参数
/// </summary>
public class SysOrgEditInput : SysOrgAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 组织复制参数
/// </summary>
public class SysOrgCopyInput
{
    /// <summary>
    /// 目标ID
    /// </summary>
    public long TargetId { get; set; }

    /// <summary>
    /// 组织Id列表
    /// </summary>
    [Required(ErrorMessage = "Ids列表不能为空")]
    public List<long>? Ids { get; set; }

    /// <summary>
    /// 是否包含下级
    /// </summary>
    public bool ContainsChild { get; set; } = false;
    
    /// <summary>
    /// 是否包含职位
    /// </summary>
    public bool ContainsPosition { get; set; } = false;
}

/// <summary>
/// 组织导入
/// </summary>
public class SysOrgImportInput : ImportTemplateInput
{
    /// <summary>
    /// 名称
    ///</summary>
    [ImporterHeader(Name = "名称")]
    [Required(ErrorMessage = "名称不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 上级组织
    ///</summary>
    [ImporterHeader(Name = "上级组织")]
    [Required(ErrorMessage = "上级组织不能为空")]
    public string Names { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    [ImporterHeader(Name = "分类")]
    [Required(ErrorMessage = "分类不能为空")]
    public string Category { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [ImporterHeader(Name = "排序码")]
    public int SortCode { get; set; } = 1;

    /// <summary>
    /// 主管账号
    ///</summary>
    [ImporterHeader(Name = "主管账号")]
    [Required(ErrorMessage = "主管账号不能为空")]
    public string Director { get; set; }
}

/// <summary>
/// 组织树查询参数
/// 懒加载用
/// </summary>
public class SysOrgTreeInput
{
    /// <summary>
    /// 父Id
    /// </summary>
    public long? ParentId { get; set; }
}
