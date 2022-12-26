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
