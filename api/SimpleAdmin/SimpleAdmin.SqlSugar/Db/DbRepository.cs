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
/// 仓储模式对象
/// </summary>
[SuppressSniffer]
public class DbRepository<T> : SimpleClient<T> where T : class, new()
{
    protected ITenant Tenant;//多租户事务、GetConnection、IsAnyConnection等功能

    public DbRepository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
    {
        Context = DbContext.DB.GetConnectionScopeWithAttr<T>();//ioc注入的对象
        Tenant = DbContext.DB;
    }

    #region 仓储方法拓展

    #region 插入

    /// <summary>
    /// 批量插入判断走普通导入还是大数据
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="threshold">阈值</param>
    /// <returns></returns>
    public virtual async Task<int> InsertOrBulkCopy(List<T> data, int threshold = 10000)
    {
        if (data.Count > threshold)
            return await Context.Fastest<T>().BulkCopyAsync(data);//大数据导入
        return await Context.Insertable(data).ExecuteCommandAsync();//普通导入
    }

    #endregion 插入

    #region 列表

    /// <summary>
    /// 获取列表指定多个字段
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="selectExpression">查询字段</param>
    /// <returns></returns>
    public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, T>> selectExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).Select(selectExpression).ToListAsync();
    }

    /// <summary>
    /// 获取列表指定单个字段
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="selectExpression">查询字段</param>
    /// <returns></returns>
    public virtual Task<List<string>> GetListAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, string>> selectExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).Select(selectExpression).ToListAsync();
    }

    /// <summary>
    /// 获取列表指定单个字段
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="selectExpression">查询字段</param>
    /// <returns></returns>
    public virtual Task<List<long>> GetListAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, long>> selectExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).Select(selectExpression).ToListAsync();
    }

    #endregion 列表

    #region 单查

    /// <summary>
    /// 获取指定表的单个字段
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="selectExpression">查询字段</param>
    /// <returns></returns>
    public virtual Task<string> GetFirstAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, string>> selectExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).Select(selectExpression).FirstAsync();
    }

    /// <summary>
    /// 获取指定表的单个字段
    /// </summary>
    /// <param name="whereExpression">查询条件</param>
    /// <param name="selectExpression">查询字段</param>
    /// <returns></returns>
    protected virtual Task<long> GetFirstAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, long>> selectExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).Select(selectExpression).FirstAsync();
    }

    /// <summary>
    /// 根据条件查询获取自动分表的单个数据
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>实体</returns>
    public virtual Task<T> GetFirstSplitTableAsync(Expression<Func<T, bool>> whereExpression, DateTime startTime, DateTime endTime)
    {
        return Context.Queryable<T>().Where(whereExpression).SplitTable(startTime, endTime).FirstAsync();
    }

    /// <summary>
    /// 根据条件查询获取自动分表的单个数据
    /// </summary>
    /// <param name="whereExpression">条件表达式</param>
    /// <param name="getTableNamesFunc">分表查询表达式</param>
    /// <returns>实体</returns>
    public virtual Task<T> GetFirstSplitTableAsync(Expression<Func<T, bool>> whereExpression,
        Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc)
    {
        return Context.Queryable<T>().Where(whereExpression).SplitTable(getTableNamesFunc).FirstAsync();
    }

    #endregion 单查

    #endregion 仓储方法拓展
}
