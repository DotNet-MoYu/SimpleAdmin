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
    /// 获取字典
    /// </summary>
    /// <param name="DictValue">字典</param>
    /// <returns></returns>
    Task<DevDict> GetDict(string DictValue);

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns>字典列表</returns>
    Task<List<DevDict>> GetListAsync();

    /// <summary>
    /// 根据字典DictValue获取字典值列表
    /// </summary>
    /// <param name="DictValue">字典值</param>
    /// <param name="devDictList">字典列表</param>
    /// <returns>字典值列表</returns>
    Task<List<string>> GetValuesByDictValue(string DictValue, List<DevDict> devDictList = null);

    /// <summary>
    /// 根据字典DictValue列表获取对应字典值列表
    /// </summary>
    /// <param name="DictValues">字典值列表</param>
    /// <returns></returns>
    Task<Dictionary<string, List<string>>> GetValuesByDictValue(string[] DictValues);

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
