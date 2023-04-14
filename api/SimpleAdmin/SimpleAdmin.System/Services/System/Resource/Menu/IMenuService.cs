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
    List<SysResource> ConstructMenuTrees(List<SysResource> resourceList, long parentId = 0);

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <param name="input">菜单树查询参数</param>
    /// <returns>菜单树列表</returns>
    Task<List<SysResource>> Tree(MenuTreeInput input);

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
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 改变菜单模块
    /// </summary>
    /// <param name="input">改变菜单模块参数</param>
    /// <returns></returns>
    Task ChangeModule(MenuChangeModuleInput input);
}