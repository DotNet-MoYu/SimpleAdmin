namespace SimpleAdmin.System;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class DevConfigSeedData : ISqlSugarEntitySeedData<DevConfig>
{
    public IEnumerable<DevConfig> SeedData()
    {
        return SeedDataUtil.GetSeedData<DevConfig>("seed_dev_config.json");
    }
}