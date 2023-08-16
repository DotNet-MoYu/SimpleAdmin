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
    Task<List<SysResource>> GetaModuleAndMenuAndSpaList();

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
    /// <param name="categorys">资源分类列表</param>
    /// <returns></returns>
    Task<List<SysResource>> GetListAsync(List<string>? categorys = null);

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
}