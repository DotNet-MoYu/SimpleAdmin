namespace SimpleAdmin.System;

/// <summary>
/// 模块管理服务
/// </summary>
public interface IModuleService : ITransient
{
    /// <summary>
    /// 添加模块
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(ModuleAddInput input);

    /// <summary>
    /// 删除模块
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 编辑模块
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(ModuleEditInput input);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="input">分页参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysResource>> Page(ModulePageInput input);
}