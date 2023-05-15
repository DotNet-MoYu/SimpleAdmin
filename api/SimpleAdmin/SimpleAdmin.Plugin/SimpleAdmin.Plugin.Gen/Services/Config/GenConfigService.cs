namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// <inheritdoc cref="IGenConfigService"/>
/// </summary>
public class GenConfigService : DbRepository<GenConfig>, IGenConfigService
{
    private readonly ILogger<GenConfigService> logger;

    public GenConfigService(ILogger<GenConfigService> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<GenConfig>> List(long basicId)
    {
        var configs = await GetListAsync(it => it.BasicId == basicId);//获取配置表相关配置
        configs = configs.OrderBy(it => it.FieldIndex).ToList();//排序一下
        return configs;
    }

    /// <inheritdoc/>
    public async Task EditBatch(List<GenConfig> configs)
    {
        await UpdateRangeAsync(configs);//批量更新
    }
}