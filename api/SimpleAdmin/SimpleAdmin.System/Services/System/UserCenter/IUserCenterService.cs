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
    Task UpdateUserInfo(UpdateInfoInput input);

    /// <summary>
    /// 更新签名
    /// </summary>
    /// <param name="input">签名图片</param>
    /// <returns></returns>
    Task UpdateSignature(UpdateSignatureInput input);

    /// <summary>
    /// 获取组织架构
    /// </summary>
    /// <returns>组织架构</returns>
    Task<List<LoginOrgTreeOutput>> LoginOrgTree();

    /// <summary>
    /// 编辑个人工作台
    /// </summary>
    /// <param name="input">工作台字符串</param>
    /// <returns></returns>
    Task UpdateWorkbench(UpdateWorkbenchInput input);
}
