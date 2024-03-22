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
/// SqlSugar分页拓展类
/// </summary>
public static class SqlSugarPageExtension
{
    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageNum, int pageSize)
    {
        var total = 0;
        var list = queryable.ToPageList(pageSize, pageNum, ref total);
        var pages = (int)Math.Ceiling(total / (double)pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            PageNum = pageSize,
            PageSize = pageNum,
            List = list,
            Total = total,
            Pages = pages,
            HasNextPages = pageSize < pages,
            HasPrevPages = pageSize - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable, int pageNum, int pageSize)
    {
        RefAsync<int> totalCount = 0;
        var list = await queryable.ToPageListAsync(pageNum, pageSize, totalCount);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            PageNum = pageNum,
            PageSize = pageSize,
            List = list,
            Total = (int)totalCount,
            Pages = totalPages,
            HasNextPages = pageNum < totalPages,
            HasPrevPages = pageNum - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static SqlSugarPagedList<TResult> ToPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, int pageNum, int pageSize,
        Expression<Func<TEntity, TResult>> expression)
    {
        var totalCount = 0;
        var items = queryable.ToPageList(pageNum, pageSize, ref totalCount, expression);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new SqlSugarPagedList<TResult>
        {
            PageNum = pageNum,
            PageSize = pageSize,
            List = items,
            Total = totalCount,
            Pages = totalPages,
            HasNextPages = pageNum < totalPages,
            HasPrevPages = pageNum - 1 > 0
        };
    }

    /// <summary>
    /// SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TResult>> ToPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, int pageNum,
        int pageSize,
        Expression<Func<TEntity, TResult>> expression)
    {
        RefAsync<int> totalCount = 0;
        var items = await queryable.ToPageListAsync(pageNum, pageSize, totalCount, expression);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new SqlSugarPagedList<TResult>
        {
            PageNum = pageNum,
            PageSize = pageSize,
            List = items,
            Total = (int)totalCount,
            Pages = totalPages,
            HasNextPages = pageNum < totalPages,
            HasPrevPages = pageNum - 1 > 0
        };
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">数据列表</param>
    /// <param name="pageNum">当前页</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>分页集合</returns>
    public static LinqPagedList<T> LinqPagedList<T>(this List<T> list, int pageNum, int pageSize)
    {
        var result = list.ToPagedList(pageNum, pageSize);//获取分页
        //格式化
        return new LinqPagedList<T>
        {
            PageNum = pageNum,
            PageSize = result.PageSize,
            List = result.Data,
            Total = result.TotalCount,
            Pages = result.TotalPages,
            HasNextPages = result.HasNext,
            HasPrevPages = result.HasPrev
        };
    }
}

/// <summary>
/// SqlSugar 分页泛型集合
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarPagedList<TEntity>
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int Pages { get; set; }

    /// <summary>
    /// 当前页集合
    /// </summary>
    public IEnumerable<TEntity> List { get; set; }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevPages { get; set; }

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPages { get; set; }
}
