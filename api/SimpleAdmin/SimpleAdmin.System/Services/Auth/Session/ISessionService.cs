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
    /// <param name="input">用户ID</param>
    Task ExitSession(BaseIdInput input);

    /// <summary>
    /// 强退token
    /// </summary>
    /// <param name="input">token列表</param>
    Task ExitToken(ExitTokenInput input);

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