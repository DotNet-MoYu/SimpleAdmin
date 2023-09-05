// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

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
    Task Add(SysOrgAddInput input, string name = SimpleAdminConst.SYS_ORG);

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
    Task Edit(SysOrgEditInput input, string name = SimpleAdminConst.SYS_ORG);

    #endregion 编辑

    #region 删除

    /// <summary>
    /// 删除组织
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input, string name = SimpleAdminConst.SYS_ORG);

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
