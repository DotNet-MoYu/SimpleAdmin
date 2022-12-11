namespace SimpleAdmin.System;

/// <summary>
/// 职位表种子数据
/// </summary>
public class SysPositionSeedData : ISqlSugarEntitySeedData<SysPosition>
{
    public IEnumerable<SysPosition> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysPosition>("sys_position.json");
    }
}
