using Masuit.Tools.Models;

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// Sqlsugar分页拓展类
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
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(
        this ISugarQueryable<TEntity> queryable, int pageNum,
        int pageSize)
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
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(
        this ISugarQueryable<TEntity> queryable,
        int pageNum, int pageSize)
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
    public static SqlSugarPagedList<TResult> ToPagedList<TEntity, TResult>(
        this ISugarQueryable<TEntity> queryable, int pageNum,
        int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        var totalCount = 0;
        var items = queryable.ToPageList(pageNum, pageSize, ref totalCount,
            expression);
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
    public static async Task<SqlSugarPagedList<TResult>> ToPagedListAsync<TEntity, TResult>(
        this ISugarQueryable<TEntity> queryable, int pageNum, int pageSize,
        Expression<Func<TEntity, TResult>> expression)
    {
        RefAsync<int> totalCount = 0;
        var items = await queryable.ToPageListAsync(pageNum, pageSize, totalCount,
            expression);
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