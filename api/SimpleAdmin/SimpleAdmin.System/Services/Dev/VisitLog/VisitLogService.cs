namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IVisitLogService"/>
/// </summary>
public class VisitLogService : DbRepository<DevLogVisit>, IVisitLogService
{
    public VisitLogService()
    {
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevLogVisit>> Page(VisitLogPageInput input)
    {
        var query = Context.Queryable<DevLogVisit>()
                           .WhereIF(!string.IsNullOrEmpty(input.Account), it => it.OpAccount == input.Account)//根据账号查询
                           .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                           .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey) || it.OpIp.Contains(input.SearchKey))//根据关键字查询
                           .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                           .OrderBy(it => it.CreateTime, OrderByType.Desc);
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<List<VisitLogDayStatisticsOutput>> StatisticsByDay(int day)
    {
        //取最近七天
        var dayArray = Enumerable.Range(0, day).Select(it => DateTime.Now.Date.AddDays(it * -1)).ToList();
        //生成时间表
        var queryableLeft = Context.Reportable(dayArray).ToQueryable<DateTime>();
        //ReportableDateType.MonthsInLast1yea 表式近一年月份 并且queryable之后还能在where过滤
        var queryableRight = Context.Queryable<DevLogVisit>(); //声名表
        //报表查询
        var list = await Context.Queryable(queryableLeft, queryableRight, JoinType.Left, (x1, x2)
            => x2.CreateTime.Value.ToString("yyyy-MM-dd") == x1.ColumnName.ToString("yyyy-MM-dd"))
        .GroupBy((x1, x2) => x1.ColumnName)//根据时间分组
        .OrderBy((x1, x2) => x1.ColumnName)//根据时间升序排序
        .Select((x1, x2) => new VisitLogDayStatisticsOutput
        {
            LoginCount = SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.Log_LOGIN, 1, 0)), //null的数据要为0所以不能用count
            LogoutCount = SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.Log_LOGOUT, 1, 0)), //null的数据要为0所以不能用count
            Date = x1.ColumnName.ToString("yyyy-MM-dd")
        }
        )

        .ToListAsync();
        return list;
    }

    /// <inheritdoc />
    public async Task<List<VisitLogTotalCountOutput>> TotalCount()
    {
        var data = await Context.Queryable<DevLogVisit>()
            .GroupBy(it => it.Category)//根据分类分组
            .Select(it => new
            {
                Category = it.Category,//分类
                Count = SqlFunc.AggregateCount(it.Category)//数量
            }).ToListAsync();
        //定义结果数组
        List<VisitLogTotalCountOutput> visitLogTotalCounts = new List<VisitLogTotalCountOutput>
        {
            //添加登录数据
            new VisitLogTotalCountOutput
            {
                Type = EventSubscriberConst.LoginB,
                Value = data.Where(it => it.Category == CateGoryConst.Log_LOGIN).Select(it => it.Count).FirstOrDefault()
            },
            //添加登出数据
            new VisitLogTotalCountOutput
            {
                Type = EventSubscriberConst.LoginOutB,
                Value = data.Where(it => it.Category == CateGoryConst.Log_LOGOUT).Select(it => it.Count).FirstOrDefault()
            }
        };
        return visitLogTotalCounts;
    }

    /// <inheritdoc />
    public async Task Delete(string category)
    {
        await DeleteAsync(it => it.Category == category);//删除对应分类日志
    }
}