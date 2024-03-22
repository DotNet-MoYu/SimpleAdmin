// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.SqlSugar;

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
        _logger = logger;
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
