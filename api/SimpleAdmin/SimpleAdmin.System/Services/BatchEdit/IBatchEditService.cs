namespace SimpleAdmin.System;

/// <summary>
/// 批量服务
/// </summary>
public interface IBatchEditService : ITransient
{
    /// <summary>
    /// 批量分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页结果</returns>
    Task<SqlSugarPagedList<BatchEdit>> Page(BatchEditPageInput input);

    /// <summary>
    /// 添加批量
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(BatchEditAddInput input);

    /// <summary>
    /// 删除批量
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 获取需要批量修改的表
    /// </summary>
    /// <returns></returns>
    List<SqlSugarTableInfo> GetTables();

    /// <summary>
    /// 获取批量修改配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<BatchEditConfig>> ConfigList(BaseIdInput input);

    /// <summary>
    /// 配置字段
    /// </summary>
    /// <param name="input">字段信息</param>
    /// <returns></returns>
    Task Config(List<BatchEditConfigInput> input);

    /// <summary>
    /// 批量配置字段列表
    /// </summary>
    /// <param name="code">唯一编码</param>
    /// <returns>列表</returns>
    Task<List<BatchEditConfig>> Columns(string code);

    /// <summary>
    /// 获取字典配置
    /// </summary>
    /// <param name="code">唯一编码</param>
    /// <param name="columns">字段信息</param>
    /// <returns>sqlsugar对应字典</returns>
    Task<Dictionary<string, object>> GetUpdateBatchConfigDict(string code, List<BatchEditColumn> columns);

    /// <summary>
    /// 同步字段
    /// </summary>
    /// <param name="input">id</param>
    /// <returns></returns>
    Task SyncColumns(BaseIdInput input);
}