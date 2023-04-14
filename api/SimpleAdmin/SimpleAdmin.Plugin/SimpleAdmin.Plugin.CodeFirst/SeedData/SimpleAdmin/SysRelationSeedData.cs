namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 关系表种子数据
/// </summary>
public class SysRelationSeedData : ISqlSugarEntitySeedData<SysRelation>
{
    public IEnumerable<SysRelation> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysRelation>(SqlsugarConst.DB_Default, "sys_relation.json");
    }
}