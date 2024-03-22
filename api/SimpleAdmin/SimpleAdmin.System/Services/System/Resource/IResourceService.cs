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
/// 资源服务
/// </summary>
public interface IResourceService : ITransient
{
    /// <summary>
    /// 获取所有的菜单和模块以及单页面列表，并按分类和排序码排序
    /// </summary>
    /// <returns>所有的菜单和模块以及单页面列表</returns>
    Task<List<SysResource>> GetAllModuleAndMenuAndSpaList();

    /// <summary>
    ///  根据模块ID获取菜单和单页面列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<SysResource>> GetMenuAndSpaListByModuleId(long id);

    /// <summary>
    /// 根据资源ID获取所有下级资源
    /// </summary>
    /// <param name="resId">资源ID</param>
    /// <param name="isContainOneself">是否包含自己</param>
    /// <returns>资源列表</returns>
    Task<List<SysResource>> GetChildListById(long resId, bool isContainOneself = true);

    /// <summary>
    /// 根据资源ID获取所有下级资源
    /// </summary>
    /// <param name="sysResources">资源列表</param>
    /// <param name="resId">资源ID</param>
    /// <param name="isContainOneself">是否包含自己</param>
    /// <returns>资源列表</returns>
    List<SysResource> GetChildListById(List<SysResource> sysResources, long resId, bool isContainOneself = true);

    /// <summary>
    /// 获取ID获取Code列表
    /// </summary>
    /// <param name="ids">id列表</param>
    /// <param name="category">分类</param>
    /// <returns>Code列表</returns>
    Task<List<string>> GetCodeByIds(List<long> ids, string category);

    /// <summary>
    /// 获取资源列表
    /// </summary>
    /// <param name="categoryList">资源分类列表</param>
    /// <returns></returns>
    Task<List<SysResource>> GetListAsync(List<string>? categoryList = null);

    /// <summary>
    /// 根据分类获取资源列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>资源列表</returns>
    Task<List<SysResource>> GetListByCategory(string category);

    /// <summary>
    /// 根据菜单ID获取菜单
    /// </summary>
    /// <param name="ids">id列表</param>
    /// <param name="category">分类</param>
    /// <returns></returns>
    Task<List<SysResource>> GetResourcesByIds(List<long> ids, string category);

    /// <summary>
    /// 获取权限授权树
    /// </summary>
    /// <param name="routes">路由列表</param>
    /// <returns></returns>
    List<PermissionTreeSelector> PermissionTreeSelector(List<string> routes);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns></returns>
    Task RefreshCache(string category = null);

    /// <summary>
    /// 角色授权资源树
    /// </summary>
    /// <returns></returns>
    Task<List<ResTreeSelector>> ResourceTreeSelector();

    /// <summary>
    /// 获取上级
    /// </summary>
    /// <param name="resourceList"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    List<SysResource> GetResourceParent(List<SysResource> resourceList, long parentId);

    /// <summary>
    /// 获取授权菜单类菜单名称
    /// </summary>
    /// <param name="menuList">菜单列表</param>
    /// <param name="menu">当前菜单</param>
    /// <returns></returns>
    string GetRoleGrantResourceMenuTitle(List<SysResource> menuList, SysResource menu);
}
