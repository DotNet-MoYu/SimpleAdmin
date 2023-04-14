namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// <inheritdoc cref="IGenConfigSerivce"/>
/// </summary>
public class GenConfigSerivce : DbRepository<GenConfig>, IGenConfigSerivce
{
    private readonly ILogger<GenConfigSerivce> logger;

    public GenConfigSerivce(ILogger<GenConfigSerivce> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<GenConfig>> List(long basicId)
    {
        var configs = await GetListAsync(it => it.BasicId == basicId);//获取配置表相关配置
        return configs.OrderBy(it => it.SortCode).ToList();//排序一下
    }

    /// <inheritdoc/>
    public async Task EditBatch(List<GenConfig> configs)
    {
        await UpdateRangeAsync(configs);//批量更新
    }
}