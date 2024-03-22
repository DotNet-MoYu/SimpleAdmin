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
/// 角色授权资源树输出
/// </summary>
public class ResTreeSelector
{
    /// <summary>
    /// 模块id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 模块图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 模块下菜单集合
    /// </summary>
    public List<RoleGrantResourceMenu> Menu { get; set; }

    /// <summary>
    /// 授权菜单类
    /// </summary>
    public class RoleGrantResourceMenu
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 父名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模块id
        /// </summary>
        public long Module { get; set; }

        /// <summary>
        /// 菜单下按钮集合
        /// </summary>
        public List<RoleGrantResourceButton> Button { get; set; } = new List<RoleGrantResourceButton>();
    }

    /// <summary>
    /// 角色授权资源按钮信息
    /// </summary>
    public class RoleGrantResourceButton
    {
        /// <summary>
        /// 按钮id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}

/// <summary>
///  权限树选择器输出
/// </summary>
public class PermissionTreeSelector
{
    /// <summary>
    /// 接口描述
    /// </summary>
    public string ApiName { get; set; }

    /// <summary>
    /// 路由名称
    /// </summary>
    public string ApiRoute { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    public string PermissionName { get; set; }
}
