namespace SimpleAdmin.System;

/// <summary>
/// 字典服务
/// </summary>
public interface IDictService : ITransient
{
    /// <summary>
    /// 添加字典
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <returns></returns>
    Task Add(DictAddInput input);

    /// <summary>
    /// 构建字典树形结构
    /// </summary>
    /// <param name="dictList">字典列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns>字典树形结构</returns>
    List<DevDict> ConstructResourceTrees(List<DevDict> dictList, long parentId = 0);

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(DictDeleteInput input);

    /// <summary>
    /// 编辑字典
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <returns></returns>
    Task Edit(DictAddInput input);

    /// <summary>
    /// 字典分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>字典分页列表</returns>
    Task<SqlSugarPagedList<DevDict>> Page(DictPageInput input);

    /// <summary>
    /// 获取字典树形结构
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>字典树形结构</returns>
    Task<List<DevDict>> Tree(DictTreeInput input);
}
