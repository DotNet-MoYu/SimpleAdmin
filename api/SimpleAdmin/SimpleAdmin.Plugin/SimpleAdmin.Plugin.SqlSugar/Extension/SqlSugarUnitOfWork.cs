using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// SqlSugar 事务和工作单元
/// </summary>
public sealed class SqlSugarUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// SqlSugar 对象
    /// </summary>
    private readonly ISqlSugarClient _sqlSugarClient;

    private readonly ILogger<SqlSugarUnitOfWork> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    /// <param name="logger"></param>
    public SqlSugarUnitOfWork(ISqlSugarClient sqlSugarClient, ILogger<SqlSugarUnitOfWork> logger)
    {
        _sqlSugarClient = sqlSugarClient;
        this._logger = logger;
    }

    /// <summary>
    /// 开启工作单元处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BeginTransaction(FilterContext context, UnitOfWorkAttribute unitOfWork)
    {
        _sqlSugarClient.AsTenant().BeginTran();
    }

    /// <summary>
    /// 提交工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void CommitTransaction(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        _sqlSugarClient.AsTenant().CommitTran();
    }

    /// <summary>
    /// 回滚工作单元处理
    /// </summary>
    /// <param name="resultContext"></param>
    /// <param name="unitOfWork"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void RollbackTransaction(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
    {
        _sqlSugarClient.AsTenant().RollbackTran();
    }

    /// <summary>
    /// 执行完毕（无论成功失败）
    /// </summary>
    /// <param name="context"></param>
    /// <param name="resultContext"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnCompleted(FilterContext context, FilterContext resultContext)
    {
        _sqlSugarClient.Dispose();
    }
}