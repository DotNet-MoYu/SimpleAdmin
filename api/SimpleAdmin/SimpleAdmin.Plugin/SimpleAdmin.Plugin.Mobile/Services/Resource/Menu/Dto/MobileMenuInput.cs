﻿// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Mobile;

/// <summary>
/// 添加菜单参数
/// </summary>
public class MobileMenuAddInput : MobileResource
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
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Icon不能为空")]
    public override string Icon { get; set; }
}

/// <summary>
/// 编辑菜单输入参数
/// </summary>
public class MobileMenuEditInput : MobileMenuAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}
