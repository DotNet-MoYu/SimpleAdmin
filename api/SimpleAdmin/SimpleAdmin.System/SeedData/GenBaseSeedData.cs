namespace SimpleAdmin.System;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class GenBaseSeedData : ISqlSugarEntitySeedData<GenBasic>
{
    public IEnumerable<GenBasic> SeedData()
    {
        return SeedDataUtil.GetSeedData<GenBasic>("seed_gen_basic.json");
    }
}
