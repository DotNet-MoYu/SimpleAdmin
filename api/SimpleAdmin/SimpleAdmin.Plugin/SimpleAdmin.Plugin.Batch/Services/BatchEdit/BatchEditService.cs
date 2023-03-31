
namespace SimpleAdmin.Plugin.Batch;

/// <summary>
/// <inheritdoc cref="IBatchEditService"/>
/// </summary>
public class BatchEditService : DbRepository<BatchEdit>, IBatchEditService
{
    private readonly ILogger<BatchEditService> _logger;

    public BatchEditService(ILogger<BatchEditService> logger)
    {
        this._logger = logger;
    }


    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<BatchEdit>> Page(BatchEditPageInput input)
    {
        var query = Context.Queryable<BatchEdit>()
                         .WhereIF(!string.IsNullOrWhiteSpace(input.ConfigId), it => it.ConfigId.Contains(input.ConfigId.Trim()))
                         .WhereIF(!string.IsNullOrWhiteSpace(input.Entityname), expression: it => it.EntityName.Contains(input.Entityname.Trim()))
                         .WhereIF(!string.IsNullOrWhiteSpace(input.Tablename), it => it.TableName.Contains(input.Tablename.Trim()))
                         //.WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                         ;
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(BatchEditAddInput input)
    {
        var entity = input.Adapt<BatchEdit>();//实体转换
        await CheckInput(entity);//检查参数
        var tableColumns = SqlSugarUtils.GetTableColumns(input.ConfigId, input.TableName);//获取表的字段信息
        List<BatchEditConfig> batchEdiConfig = new List<BatchEditConfig>();//代码生成配置字段集合

        //遍历字段
        tableColumns.ForEach(it =>
        {
            //判断是否是主键或者通用字段
            var isPkOrCommon = it.IsPrimarykey || SqlSugarUtils.IsCommonColumn(it.ColumnName);
            if (!isPkOrCommon)
            {
                //添加到字段集合
                batchEdiConfig.Add(GetUpdateBatchConfig(it));
            }
        });
        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            entity = await InsertReturnEntityAsync(entity);//输入参数转实体并插入
            batchEdiConfig.ForEach(it => { it.UId = entity.Id; });//遍历字段赋值基础Id
            await Context.Insertable(batchEdiConfig).ExecuteCommandAsync();

        });
        if (!result.IsSuccess)//如果失败了
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }

    }

    /// <inheritdoc />
    public async Task Config(List<BatchEditConfigInput> input)
    {
        var updateBatch = input.Adapt<List<BatchEditConfig>>();//实体转换
        var configRep = ChangeRepository<DbRepository<BatchEditConfig>>();
        var ids = input.Select(it => it.Id).ToList();//获取当前配置Id
        if (ids.Any())
        {
            await configRep.DeleteAsync(it => !ids.Contains(it.Id) && it.UId == input.First().UId);//删除没有的
            await Context.Updateable(updateBatch).ExecuteCommandAsync();//更新数据
        }

    }


    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //事务  
            var result = await itenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除数据
                await Context.Deleteable<BatchEditConfig>().Where(it => ids.Contains(it.UId)).ExecuteCommandAsync();

            });
            if (!result.IsSuccess)//如果失败了
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0003);
            }

        }
    }

    /// <inheritdoc />
    public async Task SyncColumns(BaseIdInput input)
    {
        var config = await GetFirstAsync(it => it.Id == input.Id);
        if (config != null)
        {
            List<BatchEditConfig> newColumns = new List<BatchEditConfig>();
            //获取表的字段信息
            var tableColumns = SqlSugarUtils.GetTableColumns(config.ConfigId, config.TableName);
            //找到当前配置字段列表
            var batchEdiConfig = await Context.Queryable<BatchEditConfig>().Where(it => it.UId == config.Id).ToListAsync();
            foreach (var tableColumn in tableColumns)
            {
                //判断是否是主键或者通用字段
                var isPkOrCommon = tableColumn.IsPrimarykey || SqlSugarUtils.IsCommonColumn(tableColumn.ColumnName);
                if (!isPkOrCommon)
                {
                    //如果当前配置没有
                    if (!batchEdiConfig.Any(it => it.ColumnName == tableColumn.ColumnName))
                    {
                        var netType = SqlSugarUtils.ConvertDataType(tableColumn.DataType);
                        //添加到字段集合
                        newColumns.Add(GetUpdateBatchConfig(tableColumn));
                    }
                }

            }
            if (newColumns.Count > 0)
            {
                newColumns.ForEach(it => it.UId = config.Id);
                await Context.Insertable(newColumns).ExecuteCommandAsync();//插入新的字段数据
            }
        }

    }

    /// <inheritdoc />
    public async Task<List<BatchEditConfig>> Columns(string code)
    {
        List<BatchEditConfig> batchEdiConfig = new List<BatchEditConfig>();
        var updateBatch = await GetFirstAsync(it => it.Code == code);//根据code获取配置
        if (updateBatch != null)
        {
            //找到对应字段
            batchEdiConfig = await Context.Queryable<BatchEditConfig>().Where(it => it.UId == updateBatch.Id && it.Status == DevDictConst.COMMON_STATUS_ENABLE).ToListAsync();
        }
        return batchEdiConfig;


    }


    /// <inheritdoc/>
    public List<SqlSugarTableInfo> GetTables()
    {
        return SqlSugarUtils.GetTablesByAttribute<BatchEditAttribute>();
    }

    /// <inheritdoc/>
    public async Task<List<BatchEditConfig>> ConfigList(BaseIdInput input)
    {
        return await Context.Queryable<BatchEditConfig>().Where(u => u.UId == input.Id).OrderByDescending(it => it.Status).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> GetUpdateBatchConfigDict(string code, List<BatchEditColumn> columns)
    {
        var dic = new Dictionary<string, object>();
        var configs = await Columns(code);
        foreach (var item in columns)
        {
            var config = configs.Where(it => it.ColumnName == item.TableColumn).FirstOrDefault();
            if (config == null) throw Oops.Bah("不存在的列");
            dic.Add(item.TableColumn, item.ColumnValue.ToString());

        }
        return dic;
    }




    #region 方法

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="columnInfo"></param>
    private BatchEditConfig GetUpdateBatchConfig(SqlsugarColumnInfo columnInfo)
    {
        var netType = SqlSugarUtils.ConvertDataType(columnInfo.DataType);
        return new BatchEditConfig
        {
            ColumnName = columnInfo.ColumnName,
            ColumnComment = string.IsNullOrWhiteSpace(columnInfo.ColumnDescription) ? columnInfo.ColumnName : columnInfo.ColumnDescription,
            NetType = netType,
            DataType = SqlSugarUtils.DataTypeToEff(netType),
            Status = DevDictConst.COMMON_STATUS_DISABLED
        };
    }

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="updateBatch"></param>
    private async Task CheckInput(BatchEdit updateBatch)
    {
        if (updateBatch.Id == 0)
        {
            var isExist = await IsAnyAsync(it => it.Code == updateBatch.Code);
            if (isExist) throw Oops.Bah("唯一编码不能重复");
        }
    }


    #endregion

}


