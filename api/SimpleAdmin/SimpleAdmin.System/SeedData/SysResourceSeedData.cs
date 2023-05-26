namespace SimpleAdmin.System;

/// <summary>
/// 资源表种子数据
/// </summary>
public class SysResourceSeedData : ISqlSugarEntitySeedData<SysResource>
{
    public IEnumerable<SysResource> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysResource>("seed_sys_resource.json");
    }
}