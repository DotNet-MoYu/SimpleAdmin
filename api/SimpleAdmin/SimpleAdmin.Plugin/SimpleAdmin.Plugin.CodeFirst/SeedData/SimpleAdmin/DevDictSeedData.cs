namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 字典表种子数据
/// </summary>
public class DevDictSeedData : ISqlSugarEntitySeedData<DevDict>
{
    public IEnumerable<DevDict> SeedData()
    {
        return SeedDataUtil.GetSeedData<DevDict>(SqlsugarConst.DB_Default, "dev_dict.json");
    }
}
