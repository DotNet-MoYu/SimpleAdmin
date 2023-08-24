namespace SimpleAdmin.Application;

/// <summary>
/// 机构服务
/// </summary>
public interface IOrgService : ITransient
{
    /// <summary>
    /// 添加机构
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(SysOrgAddInput input);

    /// <summary>
    /// 复制机构
    /// </summary>
    /// <param name="input">机构复制参数</param>
    /// <returns></returns>
    Task Copy(SysOrgCopyInput input);

    /// <summary>
    /// 删除机构
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 编辑机构
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(SysOrgEditInput input);

    /// <summary>
    /// 机构分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页信息</returns>
    Task<SqlSugarPagedList<SysOrg>> Page(SysOrgPageInput input);

    /// <summary>
    /// 机构树结构
    /// </summary>
    /// <param name="input">机构选择器</param>
    /// <returns></returns>
    Task<List<SysOrg>> Tree(SysOrgTreeInput input = null);

    /// <summary>
    /// 机构详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SysOrg> Detail(BaseIdInput input);
}