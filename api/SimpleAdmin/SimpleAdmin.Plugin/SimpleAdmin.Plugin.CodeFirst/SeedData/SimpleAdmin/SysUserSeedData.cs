

namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 用户表种子数据
/// </summary>
public class SysUserSeedData : ISqlSugarEntitySeedData<SysUser>
{
    public IEnumerable<SysUser> SeedData()
    {
        return SeedDataUtil.GetSeedData<SysUser>(SqlsugarConst.DB_Default, "sys_user.json");
    }
}
