namespace SimpleAdmin.System;

/// <summary>
/// 字典表种子数据
/// </summary>
public class DevDictSeedData : ISqlSugarEntitySeedData<DevDict>
{
    public IEnumerable<DevDict> SeedData()
    {
        return SeedDataUtil.GetSeedData<DevDict>("seed_dev_dict.json");
    }
}