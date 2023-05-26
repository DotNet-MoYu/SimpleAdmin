namespace SimpleAdmin.System;

/// <summary>
/// 角色种子数据
/// </summary>
public class SysRoleSeedData : ISqlSugarEntitySeedData<SysRole>
{
    public IEnumerable<SysRole> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysRole>("seed_sys_role.json");
    }
}