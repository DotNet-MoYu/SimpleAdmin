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
    Task Add(OrgAddInput input);

    /// <summary>
    /// 删除机构
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);


    /// <summary>
    /// 编辑机构
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(OrgEditInput input);

    /// <summary>
    /// 机构分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页信息</returns>
    Task<SqlSugarPagedList<SysOrg>> Page(OrgPageInput input);

    /// <summary>
    /// 机构树结构
    /// </summary>
    /// <returns></returns>
    Task<List<SysOrg>> Tree();
}
