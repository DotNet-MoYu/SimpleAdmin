using Microsoft.Extensions.Logging;
using Masuit.Tools;


namespace SimpleAdmin.Application;

/// <summary>
/// <inheritdoc cref="IGenTestService"/>
/// </summary>
public class GenTestService : DbRepository<GenTest>, IGenTestService
{
    private readonly ILogger<GenTestService> _logger;
    private readonly IFileService _fileService;
    private readonly IDictService _dictService;
    private readonly IImportExportService _importExportService;

    public GenTestService(ILogger<GenTestService> logger, IFileService fileService, IDictService dictService, IImportExportService importExportService)
    {
        this._logger = logger;
        this._fileService = fileService;
        this._dictService = dictService;
        this._importExportService = importExportService;
    }


    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<GenTest>> Page(GenTestPageInput input)
    {
        var query = GetQuery(input);
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(GenTestAddInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await InsertAsync(genTest);//插入数据
    }

    /// <inheritdoc />
    public async Task Edit(GenTestEditInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await UpdateAsync(genTest);//更新数据
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除数据
        }
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<GenTest> Detail(BaseIdInput input)
    {
        var genTest = await GetFirstAsync(it => it.Id == input.Id);
        return genTest;

    }

    public async Task<FileStreamResult> Template()
    {
        var templateName = "学生信息";
        //var result = _importExportService.GenerateLocalTemplate(templateName);
        var result = await _importExportService.GenerateTemplate<GenTestImportInput>(templateName);
        return result;

    }


    /// <inheritdoc/>
    public async Task<dynamic> Preview(ImportPreviewInput input)
    {
        var importPreview = await _importExportService.GetImportPreview<GenTestImportInput>(input.File);
        importPreview.Data = await CheckImport(importPreview.Data);
        return importPreview;
    }


    /// <inheritdoc/>
    public async Task<ImportResultOutPut<GenTestImportInput>> Import(ImportResultInput<GenTestImportInput> input)
    {

        var data = await CheckImport(input.Data, true);
        var result = _importExportService.GetImportResultPreview(data, out List<GenTestImportInput> importData);
        var genTests = importData.Adapt<List<GenTest>>();//转实体
        //await InsertRangeAsync(genTests);//导入用户
        //DbContext.Db.Fastest<GenTest>().BulkCopy(genTests);//性能 比现有任何Bulkcopy都要快30%
        return result;

    }

    public async Task<List<GenTestImportInput>> CheckImport(List<GenTestImportInput> genTestImports, bool clearError = false)
    {
        //自己的业务
        var dicts = await _dictService.GetListAsync();

        foreach (var data in genTestImports)
        {

            if (clearError)//如果需要清除错误
            {
                data.ErrorInfo = new Dictionary<string, string>();
                data.HasError = false;
            }
            var genders = await _dictService.GetValuesByDictValue(DevDictConst.GENDER, dicts);
            if (!genders.Contains(data.Sex)) data.ErrorInfo.Add(nameof(data.Sex), $"性别只能是男和女");

            var nations = await _dictService.GetValuesByDictValue(DevDictConst.NATION, dicts);
            if (!nations.Contains(data.Nation)) data.ErrorInfo.Add(nameof(data.Nation), $"不存在的民族");
            if (data.ErrorInfo.Count > 0) data.HasError = true;//如果错误信息数量大于0则表示有错误
        }
        genTestImports = genTestImports.OrderByDescending(it => it.HasError).ToList();//排序
        return genTestImports;

    }

    /// <inheritdoc/>
    public async Task<dynamic> Export(GenTestPageInput input)
    {
        var query = GetQuery(input);
        var genTests = await query.ToListAsync();//分页
        var data = genTests.Adapt<List<GenTestExport>>();
        var result = await _importExportService.Export(data, "学生信息");
        return result;

    }
    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="genTest"></param>
    private async Task CheckInput(GenTest genTest)
    {

    }

    private ISugarQueryable<GenTest> GetQuery(GenTestPageInput input)
    {
        var query = Context.Queryable<GenTest>()
                        .WhereIF(input.Sex != null, it => it.Sex == input.Sex)
                        .WhereIF(input.Age != null, it => it.Age == input.Age)
                        .WhereIF(input.StartBir != null || input.EndBir != null, it => SqlFunc.Between(it.Bir, input.StartBir, input.EndBir))
                        .WhereIF(input.ExtJson != null, it => it.ExtJson == input.ExtJson)
                        .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                        .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                        .OrderBy(it => it.SortCode);//排序

        return query;
    }

    #endregion

}


