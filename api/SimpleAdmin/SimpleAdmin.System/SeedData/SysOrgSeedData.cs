namespace SimpleAdmin.System;

/// <summary>
/// 机构种子数据
/// </summary>
public class SysOrgSeedData : ISqlSugarEntitySeedData<SysOrg>
{
    public IEnumerable<SysOrg> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysOrg>("seed_sys_org.json");
    }
}