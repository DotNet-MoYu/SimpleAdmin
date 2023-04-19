namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// 代码生成详细配置服务
/// </summary>
public interface IGenConfigService : ITransient
{
    /// <summary>
    /// 批量更新
    /// </summary>
    /// <param name="configs"></param>
    /// <returns></returns>
    Task EditBatch(List<GenConfig> configs);

    /// <summary>
    /// 查询代码生成详细配置列表
    /// </summary>
    /// <param name="basicId"></param>
    /// <returns>配置列表</returns>
    Task<List<GenConfig>> List(long basicId);
}