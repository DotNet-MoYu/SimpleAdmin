namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 角色种子数据
/// </summary>
public class SysRoleSeedData : ISqlSugarEntitySeedData<SysRole>
{
    public IEnumerable<SysRole> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysRole>(SqlsugarConst.DB_Default, "sys_role.json");
    }
}