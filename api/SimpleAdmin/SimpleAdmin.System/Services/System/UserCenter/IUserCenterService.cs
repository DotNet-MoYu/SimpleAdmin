namespace SimpleAdmin.System;

/// <summary>
/// 个人信息中心服务
/// </summary>
public interface IUserCenterService : ITransient
{
    #region 查询

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
    /// 获取组织架构
    /// </summary>
    /// <returns>组织架构</returns>
    Task<List<LoginOrgTreeOutput>> LoginOrgTree();

    /// <summary>
    /// 获取登录用户的站内信分页
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>站内信列表</returns>
    Task<SqlSugarPagedList<DevMessage>> LoginMessagePage(MessagePageInput input);

    /// <summary>
    /// 读取登录用户站内信详情
    /// </summary>
    /// <param name="input">消息ID</param>
    /// <returns>消息详情</returns>
    Task<MessageDetailOutPut> LoginMessageDetail(BaseIdInput input);

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    /// <returns>未读消息数量</returns>
    Task<int> UnReadCount();

    #endregion 查询

    #region 编辑

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
    /// 编辑个人工作台
    /// </summary>
    /// <param name="input">工作台字符串</param>
    /// <returns></returns>
    Task UpdateWorkbench(UpdateWorkbenchInput input);

    /// <summary>
    /// 删除我的消息
    /// </summary>
    /// <param name="input">消息Id</param>
    /// <returns></returns>
    Task DeleteMyMessage(BaseIdInput input);

    /// <summary>
    /// 修改个人密码
    /// </summary>
    /// <param name="input">密码信息</param>
    /// <returns></returns>
    Task UpdatePassword(UpdatePasswordInput input);

    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="input">头像文件</param>
    /// <returns></returns>
    Task<string> UpdateAvatar(BaseFileInput input);

    /// <summary>
    /// 修改默认模块
    /// </summary>
    /// <param name="input">默认模块输入参数</param>
    /// <returns></returns>
    Task SetDeafultModule(SetDeafultModuleInput input);

    #endregion 编辑
}