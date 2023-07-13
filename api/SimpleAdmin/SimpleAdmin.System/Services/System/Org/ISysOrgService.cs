namespace SimpleAdmin.System;

/// <summary>
/// 组织架构服务
/// </summary>
public interface ISysOrgService : ITransient
{
    #region 查询

    /// <summary>
    /// 检查组织是否存在
    /// </summary>
    /// <param name="sysOrgs">组织列表</param>
    /// <param name="orgName">组织名称</param>
    /// <param name="parentId">父Id</param>
    /// <param name="orgId">组织Id</param>
    /// <returns>是否存在,存在返回组织ID</returns>
    bool IsExistOrgByName(List<SysOrg> sysOrgs, string orgName, long parentId,
        out long orgId);

    /// <summary>
    /// 组织详情
    /// </summary>
    /// <param name="input">id参数</param>
    /// <returns>组织信息</returns>
    Task<SysOrg> Detail(BaseIdInput input);

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
    Task<SqlSugarPagedList<SysOrg>> Page(SysOrgPageInput input);

    /// <summary>
    /// 根据ID列表获取组织列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<SysOrg>> GetOrgListByIdList(IdListInput input);

    #endregion 查询

    #region 新增

    /// <summary>
    /// 添加组织
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Add(SysOrgAddInput input, string name = SimpleAdminConst.SysOrg);

    /// <summary>
    /// 复制组织
    /// </summary>
    /// <param name="input">机构复制参数</param>
    /// <returns></returns>
    Task Copy(SysOrgCopyInput input);

    #endregion 新增

    #region 编辑

    /// <summary>
    /// 编辑组织
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Edit(SysOrgEditInput input, string name = SimpleAdminConst.SysOrg);

    #endregion 编辑

    #region 删除

    /// <summary>
    /// 删除组织
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input, string name = SimpleAdminConst.SysOrg);

    #endregion 删除

    #region 其他

    /// <summary>
    /// 构建组织树形结构
    /// </summary>
    /// <param name="orgList">组织列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns>树型结构</returns>
    List<SysOrg> ConstrucOrgTrees(List<SysOrg> orgList, long parentId = 0);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    Task RefreshCache();

    /// <summary>
    /// 获取组织树型结构
    /// </summary>
    /// <param name="orgIds">机构ID列表</param>
    /// <param name="treeInput">组织选择器(懒加载用)</param>
    /// <returns>组织树列表</returns>
    Task<List<SysOrg>> Tree(List<long> orgIds = null, SysOrgTreeInput treeInput = null);

    #endregion 其他
}