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
/// 会话输出
/// </summary>
public class SessionOutput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// 头像
    ///</summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 账号
    ///</summary>
    public virtual string Account { get; set; }

    /// <summary>
    /// 姓名
    ///</summary>

    public virtual string Name { get; set; }

    /// <summary>
    /// 在线状态
    /// </summary>
    public string OnlineStatus { get; set; }

    /// <summary>
    /// 最新登录ip
    ///</summary>
    public string LatestLoginIp { get; set; }

    /// <summary>
    /// 最新登录地点
    ///</summary>
    public string LatestLoginAddress { get; set; }

    /// <summary>
    /// 最新登录时间
    ///</summary>
    public DateTime? LatestLoginTime { get; set; }

    /// <summary>
    /// 令牌数量
    /// </summary>
    public int TokenCount { get; set; }

    /// <summary>
    /// 令牌信息集合
    /// </summary>
    public List<TokenInfo> TokenSignList { get; set; }
}

/// <summary>
/// 会话统计结果
/// </summary>
public class SessionAnalysisOutPut
{
    /// <summary>
    /// 当前会话总数量
    /// </summary>
    public int CurrentSessionTotalCount { get; set; }

    /// <summary>
    /// 最大签发令牌数
    /// </summary>
    public int MaxTokenCount { get; set; }

    /// <summary>
    /// 在线用户数
    /// </summary>
    public int OnLineCount { get; set; }

    /// <summary>
    /// BC端会话比例
    /// </summary>
    public string ProportionOfBAndC { get; set; }
}
