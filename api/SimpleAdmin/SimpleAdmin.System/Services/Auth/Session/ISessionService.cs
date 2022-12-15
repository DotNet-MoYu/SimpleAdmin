namespace SimpleAdmin.System;

/// <summary>
/// 会话管理服务
/// </summary>
public interface ISessionService : ITransient
{
    /// <summary>
    /// 会话统计
    /// </summary>
    /// <returns>统计结果</returns>
    SessionAnalysisOutPut Analysis();

    /// <summary>
    /// 强退会话
    /// </summary>
    /// <param name="input"></param>
    /// <param name="loginClientType"></param>
    void ExitSession(BaseIdInput input, LoginClientTypeEnum loginClientType);

    /// <summary>
    /// 强退token
    /// </summary>
    /// <param name="input"></param>
    /// <param name="loginClientType"></param>
    void ExitToken(ExitTokenInput input, LoginClientTypeEnum loginClientType);

    /// <summary>
    /// B端会话分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>B端会话列表</returns>
    Task<SqlSugarPagedList<SessionOutput>> PageB(SessionPageInput input);

    /// <summary>
    /// C端会话分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>C端会话列表</returns>
    Task<SqlSugarPagedList<SessionOutput>> PageC(SessionPageInput input);
}
