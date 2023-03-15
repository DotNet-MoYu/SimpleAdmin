namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IOperateLogService"/>
/// </summary>
public class OperateLogService : DbRepository<DevLogOperate>, IOperateLogService
{
    /// <summary>
    /// 操作日志中文名称
    /// </summary>
    private readonly string NameOperate = "操作日志";

    /// <summary>
    /// 异常日志中文名称
    /// </summary>
    private readonly string NameExecption = "异常日志";

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevLogOperate>> Page(OperateLogPageInput input)
    {
        var query = Context.Queryable<DevLogOperate>()
                           .WhereIF(!string.IsNullOrEmpty(input.Account), it => it.OpAccount == input.Account)//根据账号查询
                           .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                           .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey) || it.OpIp.Contains(input.SearchKey))//根据关键字查询
                           .IgnoreColumns(it => new { it.ParamJson, it.ResultJson })
                           .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                           .OrderBy(it => it.CreateTime, OrderByType.Desc);
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<List<OperateLogDayStatisticsOutput>> StatisticsByDay(int day)
    {

        //取最近七天
        var dayArray = Enumerable.Range(0, day).Select(it => DateTime.Now.Date.AddDays(it * -1)).ToList();
        //生成时间表
        var queryableLeft = Context.Reportable(dayArray).ToQueryable<DateTime>();
        //ReportableDateType.MonthsInLast1yea 表式近一年月份 并且queryable之后还能在where过滤
        var queryableRight = Context.Queryable<DevLogOperate>(); //声名表
        //报表查询
        var list = await Context.Queryable(queryableLeft, queryableRight, JoinType.Left, (x1, x2)
            => x2.CreateTime.Value.ToString("yyyy-MM-dd") == x1.ColumnName.ToString("yyyy-MM-dd"))
        .GroupBy((x1, x2) => x1.ColumnName)//根据时间分组
        .OrderBy((x1, x2) => x1.ColumnName)//根据时间升序排序
        .Select((x1, x2) => new
        {
            OperateCount = SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.Log_OPERATE, 1, 0)), //null的数据要为0所以不能用count
            ExceptionCount = SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.Log_EXCEPTION, 1, 0)), //null的数据要为0所以不能用count
            Date = x1.ColumnName.ToString("yyyy-MM-dd")
        }
        ).ToListAsync();
        //定义返回结果
        List<OperateLogDayStatisticsOutput> result = new List<OperateLogDayStatisticsOutput>();
        //遍历结果
        list.ForEach(it =>
        {
            result.Add(new OperateLogDayStatisticsOutput { Date = it.Date, Name = NameOperate, Count = it.OperateCount });//添加访问日志
            result.Add(new OperateLogDayStatisticsOutput { Date = it.Date, Name = NameExecption, Count = it.ExceptionCount });//添加异常日志

        });
        return result;
    }



    /// <inheritdoc />
    public async Task<List<OperateLogTotalCountOutpu>> TotalCount()
    {
        var data = await Context.Queryable<DevLogOperate>()
            .GroupBy(it => it.Category)//根据分类分组
            .Select(it => new
            {
                Category = it.Category,//分类
                Count = SqlFunc.AggregateCount(it.Category)//数量

            }).ToListAsync();
        //定义结果数组
        List<OperateLogTotalCountOutpu> operageLogTotalCounts = new List<OperateLogTotalCountOutpu>
        {
            //添加操作日志数据
            new OperateLogTotalCountOutpu
            {
                Type = NameOperate,
                Value = data.Where(it => it.Category == CateGoryConst.Log_OPERATE).Select(it => it.Count).FirstOrDefault()
            },
            //添加异常日志数据
            new OperateLogTotalCountOutpu
            {
                Type = NameExecption,
                Value = data.Where(it => it.Category == CateGoryConst.Log_EXCEPTION).Select(it => it.Count).FirstOrDefault()
            }
        };
        return operageLogTotalCounts;
    }

    /// <inheritdoc />
    public async Task Delete(string category)
    {
        await DeleteAsync(it => it.Category == category);//删除对应分类日志
    }

    /// <inheritdoc />
    public async Task<DevLogOperate> Detail(BaseIdInput input)
    {
        return await GetFirstAsync(it => it.Id == input.Id);//删除对应分类日志
    }

}
