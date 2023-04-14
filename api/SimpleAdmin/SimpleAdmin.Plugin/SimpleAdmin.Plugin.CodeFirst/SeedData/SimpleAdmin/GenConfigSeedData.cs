namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class GenConfigSeedData : ISqlSugarEntitySeedData<GenConfig>
{
    public IEnumerable<GenConfig> SeedData()
    {
        return SeedDataUtil.GetSeedData<GenConfig>(SqlsugarConst.DB_Default, "gen_config.json");
    }
}