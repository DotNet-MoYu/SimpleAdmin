namespace SimpleAdmin.System;

/// <summary>
/// 关系表种子数据
/// </summary>
public class SysRelationSeedData : ISqlSugarEntitySeedData<SysRelation>
{
    public IEnumerable<SysRelation> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysRelation>("seed_sys_relation.json");
    }
}