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
/// 菜单服务
/// </summary>
public interface IMenuService : ITransient
{
    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(MenuAddInput input);

    /// <summary>
    /// 详情
    /// </summary>
    /// <param name="input">id</param>
    /// <returns>详细信息</returns>
    Task<SysResource> Detail(BaseIdInput input);

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    /// <param name="resourceList">菜单列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns>菜单形结构</returns>
    List<SysResource> ConstructMenuTrees(List<SysResource> resourceList, long? parentId = 0);

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <param name="input">菜单树查询参数</param>
    /// <param name="showDisabled">是否显示禁用的</param>
    /// <returns>菜单树列表</returns>
    Task<List<SysResource>> Tree(MenuTreeInput input, bool showDisabled = true);

    /// <summary>
    /// 编辑菜单
    /// </summary>
    /// <param name="input">菜单编辑参数</param>
    /// <returns></returns>
    Task Edit(MenuEditInput input);

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="input">删除菜单参数</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 改变菜单模块
    /// </summary>
    /// <param name="input">改变菜单模块参数</param>
    /// <returns></returns>
    Task ChangeModule(MenuChangeModuleInput input);

    /// <summary>
    /// 快捷方式菜单树
    /// </summary>
    /// <param name="sysResources">资源列表</param>
    /// <returns></returns>
    Task<List<SysResource>> ShortcutTree(List<SysResource> sysResources = null);
}
