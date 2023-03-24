namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 机构种子数据
/// </summary>
public class SysOrgSeedData : ISqlSugarEntitySeedData<SysOrg>
{
    public IEnumerable<SysOrg> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysOrg>(SqlsugarConst.DB_Default, "sys_org.json");
    }
}
