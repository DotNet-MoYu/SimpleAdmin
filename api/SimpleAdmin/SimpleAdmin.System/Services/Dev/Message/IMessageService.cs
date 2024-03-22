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
/// 站内信服务
/// </summary>
public interface IMessageService : ITransient
{
    /// <summary>
    /// 删除消息
    /// </summary>
    /// <param name="input">id列表</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 删除我的消息
    /// </summary>
    /// <param name="input"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteMyMessage(BaseIdInput input, long userId);

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="input">消息ID</param>
    /// <param name="isSelf">是否是自己</param>
    /// <returns>消息详情</returns>
    Task<MessageDetailOutPut> Detail(BaseIdInput input, bool isSelf = false);

    /// <summary>
    /// 我的消息列表
    /// </summary>
    /// <param name="input"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysMessage>> MyMessagePage(MessagePageInput input, long userId);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysMessage>> Page(MessagePageInput input);

    /// <summary>
    /// 发送站内信
    /// </summary>
    /// <param name="input">站内信信息</param>
    /// <returns></returns>
    Task Send(MessageSendInput input);

    /// <summary>
    /// 获取未读消息数
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<int> UnReadCount(long userId);
}
