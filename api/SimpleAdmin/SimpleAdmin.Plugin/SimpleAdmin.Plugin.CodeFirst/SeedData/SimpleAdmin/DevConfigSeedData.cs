namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class DevConfigSeedData : ISqlSugarEntitySeedData<DevConfig>
{
    public IEnumerable<DevConfig> SeedData()
    {
        return SeedDataUtil.GetSeedData<DevConfig>(SqlsugarConst.DB_Default, "dev_config.json");
    }

}
