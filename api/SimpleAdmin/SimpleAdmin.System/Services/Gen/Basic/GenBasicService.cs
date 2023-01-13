using Furion.ViewEngine;
using Masuit.Tools;
using SimpleAdmin.System;
namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IGenbasicService"/>
/// </summary>
public class GenBasicService : DbRepository<GenBasic>, IGenbasicService
{
    private readonly ILogger<GenBasicService> _logger;
    private readonly IViewEngine _viewEngine;
    private string _sqlDir = "Sql";
    public GenBasicService(ILogger<GenBasicService> logger, IViewEngine viewEngine)
    {
        this._logger = logger;
        this._viewEngine = viewEngine;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<GenBasic>> Page(BasePageInput input)
    {

        var query = Context.Queryable<GenBasic>()
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                         .OrderBy(it => it.SortCode);
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public List<GenBasicTableOutput> GetTables()
    {
        List<GenBasicTableOutput> tables = new List<GenBasicTableOutput>();//结果集

        // 获取实体表
        var entityTypes = App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false))//有SugarTable特性
            .Where(u => u.IsDefined(typeof(CodeGenAttribute), false));//具有代码生成特性

        foreach (var entityType in entityTypes)
        {
            //获取实体信息
            var entityInfo = Context.EntityMaintenance.GetEntityInfo(entityType);
            if (entityInfo != null)
            {
                tables.Add(new GenBasicTableOutput
                {
                    TableName = entityInfo.DbTableName,
                    EntityName = entityInfo.EntityName,
                    TableRemark = entityInfo.TableDescription
                });
            }

        }
        return tables;
    }

    /// <inheritdoc/>
    public List<GenBasicColumnOutput> GetTableColumns(GenBasicColumnInput input)
    {
        List<GenBasicColumnOutput> columns = new List<GenBasicColumnOutput>();//结果集
        var dbColumnInfos = Context.DbMaintenance.GetColumnInfosByTableName(input.TableName);//根据表名获取表信息
        if (dbColumnInfos != null)
        {

            dbColumnInfos.ForEach(it =>
            {
                columns.Add(new GenBasicColumnOutput
                {
                    ColumnName = it.DbColumnName,
                    IsPrimarykey = it.IsPrimarykey,
                    ColumnRemark = it.ColumnDescription,
                    TypeName = it.DataType
                });

            });

        }
        return columns;
    }

    /// <inheritdoc/>
    public List<string> GetAssemblies()
    {
        var names = new List<string>();//结果集
        //获取所有程序集名称
        var assemblies = App.Assemblies.Where(it => it.FullName.StartsWith("SimpleAdmin")).Select(it => it.FullName).ToList();
        assemblies.ForEach(it => names.Add(it.Split(",")[0]));//根据逗号分割取第一个
        return names;
    }


    /// <inheritdoc/>
    public async Task<GenBasic> Add(GenBasicAddInput input)
    {
        var entity = input.Adapt<GenBasic>();//输入参数转实体
        var tableColumns = GetTableColumns(new GenBasicColumnInput { TableName = input.DbTable });//获取表的字段信息
        List<GenConfig> genConfigs = new List<GenConfig>();//代码生成配置字段集合
        //遍历字段
        int sortCode = 0;
        //遍历字段
        tableColumns.ForEach(it =>
        {
            #region 判断是否想显示
            var yesOrNo = GenConst.Yes;
            if (it.IsPrimarykey || CodeGenUtil.IsCommonColumn(it.ColumnName))//如果是主键或者是公共字段则不显示
                yesOrNo = GenConst.No;

            else
                yesOrNo = GenConst.Yes;
            #endregion
            //添加到字段集合
            genConfigs.Add(new GenConfig
            {
                IsPrimarykey = it.IsPrimarykey ? GenConst.Yes : GenConst.No,
                FieldName = it.ColumnName,
                FieldType = it.TypeName,
                FieldNetType = CodeGenUtil.ConvertDataType(it.TypeName),
                FieldRemark = it.ColumnRemark ?? it.ColumnName,
                EffectType = GenConst.INPUT,
                WhetherTable = yesOrNo,
                WhetherAddUpdate = yesOrNo,
                WhetherRequired = GenConst.No,
                WhetherRetract = GenConst.No,
                QueryWhether = GenConst.No,
                SortCode = yesOrNo == GenConst.No ? 99 : sortCode//如果是公共字段就排最后
            });
            sortCode++;

        });
        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            entity = await InsertReturnEntityAsync(entity);//输入参数转实体并插入
            genConfigs.ForEach(it => { it.BasicId = entity.Id; });//遍历字段赋值基础Id
            await Context.Insertable(genConfigs).ExecuteCommandAsync();

        });
        if (!result.IsSuccess)//如果失败了
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }

        return entity;
    }


    /// <inheritdoc/>
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除基础表
                await Context.Deleteable<GenConfig>().Where(it => ids.Contains(it.BasicId)).ExecuteCommandAsync();//删除配置表

            });
            if (!result.IsSuccess)//如果失败了
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }

    }

    /// <inheritdoc/>
    public async Task<GenBasic> Edit(GenBasicEditInput input)
    {
        var entity = input.Adapt<GenBasic>();//输入参数转实体
        await UpdateAsync(entity);
        return entity;

    }

    /// <inheritdoc/>
    public async Task<GenBasePreviewOutput> PreviewGen(BaseIdInput input)
    {
        var genBasic = await GetByIdAsync(input.Id);
        if (genBasic == null)
            throw Oops.Bah("代码生成配置不存在");
        var templatePath = App.WebHostEnvironment.WebRootPath + @"\CodeGen";//模板文件文件夹
        var genViewModel = GetGenViewModel(genBasic);
        var sqlResult = GetSqlCodeResult(genViewModel, templatePath);
        return new GenBasePreviewOutput { SqlResults = sqlResult };
    }
    #region 方法

    /// <summary>
    /// 获取sql代码结果
    /// </summary>
    /// <returns></returns>
    public List<GenBasePreviewOutput.GenBaseCodeResult> GetSqlCodeResult(GenViewModel genViewModel, string templatePath)
    {
        var baseCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var sqlTemplatePath = Path.Combine(templatePath, _sqlDir);//获取sql模板文件路径
        var vms = Directory.GetFiles(sqlTemplatePath);//获取文件下所有模板文件路径

        vms.ForEach(it =>
        {
            FileInfo fileInfo = new FileInfo(it);//获取文件信息
            var fileName = fileInfo.Name;//文件名
            var fileNoPrefix = fileName.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(it);//读取文件
            //视图引擎渲染
            var tResult = _viewEngine.RunCompileFromCached(tContent, genViewModel, builderAction: builder =>
            {
                builder.AddAssemblyReference(typeof(SimpleAdmin.Core.GenBasic));
            });
            baseCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileNoPrefix,
                CodeFileDir = _sqlDir
            });
        });
        return baseCodeResults;

    }

    /// <summary>
    /// 获取代码生成视图
    /// </summary>
    /// <param name="genBasic"></param>
    /// <returns></returns>
    public GenViewModel GetGenViewModel(GenBasic genBasic)
    {
        //实体转视图
        var genViewEngine = genBasic.Adapt<GenViewModel>();
        return genViewEngine;
    }
    #endregion
}
