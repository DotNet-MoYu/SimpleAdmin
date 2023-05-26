namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class GenConfigSeedData : ISqlSugarEntitySeedData<GenConfig>
{
    public IEnumerable<GenConfig> SeedData()
    {
        return SeedDataUtil.GetSeedData<GenConfig>("seed_gen_config.json");
    }
}