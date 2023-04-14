namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class GenBaseSeedData : ISqlSugarEntitySeedData<GenBasic>
{
    public IEnumerable<GenBasic> SeedData()
    {
        return SeedDataUtil.GetSeedData<GenBasic>(SqlsugarConst.DB_Default, "gen_basic.json");
    }
}