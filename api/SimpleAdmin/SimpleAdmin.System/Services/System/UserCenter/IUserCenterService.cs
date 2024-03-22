// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 个人信息中心服务
/// </summary>
public interface IUserCenterService : ITransient
{
    #region 查询

    /// <summary>
    /// 获取登录用户菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<SysResource>> GetLoginMenu(BaseIdInput input);

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
    Task<SqlSugarPagedList<SysMessage>> LoginMessagePage(MessagePageInput input);

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

    /// <summary>
    /// 获取快捷菜单树
    /// </summary>
    /// <returns></returns>
    Task<List<SysResource>> ShortcutTree();

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
    Task SetDefaultModule(SetDefaultModuleInput input);

    #endregion 编辑
}
