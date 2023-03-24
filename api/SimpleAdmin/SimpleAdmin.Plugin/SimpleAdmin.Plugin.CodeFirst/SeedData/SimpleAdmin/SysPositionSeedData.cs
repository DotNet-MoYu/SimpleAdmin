namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 职位表种子数据
/// </summary>
public class SysPositionSeedData : ISqlSugarEntitySeedData<SysPosition>
{
    public IEnumerable<SysPosition> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysPosition>(SqlsugarConst.DB_Default, "sys_position.json");
    }
}
