namespace SimpleAdmin.System;

/// <summary>
/// 个人信息中心服务
/// </summary>
public interface IUserCenterService : ITransient
{
    /// <summary>
    /// 获取个人菜单
    /// </summary>
    /// <returns></returns>
    Task<List<SysResource>> GetOwnMenu();

    /// <summary>
    /// 获取个人工作台
    /// </summary>
    /// <returns></returns>
    Task<string> GetLoginWorkbench();

    /// <summary>
    /// 更新个人信息
    /// </summary>
    /// <param name="input">信息参数</param>
    /// <returns></returns>
    Task UpdateUserInfo(UserUpdateInfoInput input);
}
