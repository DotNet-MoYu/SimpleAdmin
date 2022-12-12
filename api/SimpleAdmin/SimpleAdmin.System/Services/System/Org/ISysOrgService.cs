namespace SimpleAdmin.System;

/// <summary>
/// 组织架构服务
/// </summary>
public interface ISysOrgService : ITransient
{
    /// <summary>
    /// 添加组织
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Add(OrgAddInput input, string name = SimpleAdminConst.SysOrg);

    /// <summary>
    /// 构建组织树形结构
    /// </summary>
    /// <param name="orgList">组织列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns>树型结构</returns>
    List<SysOrg> ConstrucOrgTrees(List<SysOrg> orgList, long parentId = 0);

    /// <summary>
    /// 删除组织
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input, string name = SimpleAdminConst.SysOrg);

    /// <summary>
    /// 组织详情
    /// </summary>
    /// <param name="input">id参数</param>
    /// <returns>组织信息</returns>
    Task<SysOrg> Detail(BaseIdInput input);

    /// <summary>
    /// 编辑组织
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Edit(OrgEditInput input, string name = SimpleAdminConst.SysOrg);

    /// <summary>
    /// 根据组织ID获取 下级
    /// </summary>
    /// <param name="orgId">组织ID</param>
    /// <param name="isContainOneself">是否包含自己</param>
    /// <returns></returns>
    Task<List<SysOrg>> GetChildListById(long orgId, bool isContainOneself = true);

    /// <summary>
    /// 获取所有组织
    /// </summary>
    /// <returns>组织列表</returns>
    Task<List<SysOrg>> GetListAsync();


    /// <summary>
    /// 获取机构及下级ID列表
    /// </summary>
    /// <param name="orgId"></param>
    /// <param name="isContainOneself"></param>
    /// <returns></returns>
    Task<List<long>> GetOrgChildIds(long orgId, bool isContainOneself = true);

    /// <summary>
    /// 根据组织Id递归获取上级
    /// </summary>
    /// <param name="allOrgList">组织列表</param>
    /// <param name="orgId">组织Id</param>
    /// <param name="includeSelf">是否包含自己</param>
    /// <returns></returns>
    List<SysOrg> GetOrgParents(List<SysOrg> allOrgList, long orgId, bool includeSelf = true);

    /// <summary>
    /// 获取组织信息
    /// </summary>
    /// <param name="id">组织id</param>
    /// <returns>组织信息</returns>
    Task<SysOrg> GetSysOrgById(long id);

    /// <summary>
    /// 组织分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页信息</returns>
    Task<SqlSugarPagedList<SysOrg>> Page(OrgPageInput input);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    Task RefreshCache();

    /// <summary>
    /// 获取组织树型结构
    /// </summary>
    /// <param name="orgIds">机构ID列表</param>
    /// <returns>组织树列表</returns>
    Task<List<SysOrg>> Tree(List<long> orgIds = null);
}
