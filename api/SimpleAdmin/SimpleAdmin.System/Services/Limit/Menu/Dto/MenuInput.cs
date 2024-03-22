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
/// 菜单树查询参数
/// </summary>
public class MenuTreeInput
{
    /// <summary>
    /// 模块
    /// </summary>
    public long? Module { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string SearchKey { get; set; }
}

/// <summary>
/// 添加菜单参数
/// </summary>
public class MenuAddInput : SysResource, IValidatableObject
{
    /// <summary>
    /// 父ID
    /// </summary>
    [Required(ErrorMessage = "ParentId不能为空")]
    public override long? ParentId { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "Title不能为空")]
    public override string Title { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public override string MenuType { get; set; } = SysResourceConst.MENU;

    /// <summary>
    /// 模块
    /// </summary>
    [Required(ErrorMessage = "Module不能为空")]
    public override long? Module { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Required(ErrorMessage = "Path不能为空")]
    public override string Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Icon不能为空")]
    public override string Icon { get; set; }

    /// <summary>
    /// 特殊验证
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //如果菜单类型是菜单
        if (MenuType is SysResourceConst.MENU or SysResourceConst.SUBSET)
        {
            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name不能为空", new[] { nameof(Name) });
            if (string.IsNullOrEmpty(Component))
                yield return new ValidationResult("Component不能为空", new[] { nameof(Name) });
            if (MenuType is SysResourceConst.SUBSET)//如果是子集
            {
                if (string.IsNullOrEmpty(ActiveMenu))
                    yield return new ValidationResult("ActiveMenu不能为空", new[] { nameof(Name) });
                IsHome = false;
                IsHide = true;
                IsFull = false;
                IsAffix = false;
                IsKeepAlive = true;
            }
        }
        else
        {
            Name = null;//设置name为空
            Component = null;//设置组件为空
        }
        //设置分类为菜单
        Category = CateGoryConst.RESOURCE_MENU;
    }
}

/// <summary>
/// 编辑菜单输入参数
/// </summary>
public class MenuEditInput : MenuAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 改变模块输入参数
/// </summary>
public class MenuChangeModuleInput : BaseIdInput
{
    /// <summary>
    /// 模块ID
    /// </summary>
    [Required(ErrorMessage = "Module不能为空")]
    public long? Module { get; set; }
}
