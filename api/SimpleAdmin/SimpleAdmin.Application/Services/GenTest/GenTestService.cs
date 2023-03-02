using Furion.FriendlyException;
using Mapster;
using SimpleAdmin.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Web;
using System.Text;
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

    public GenTestService(ILogger<GenTestService> logger, IFileService fileService, IDictService dictService)
    {
        this._logger = logger;
        this._fileService = fileService;
        this._dictService = dictService;
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
        var templateName = "学生信息.xlsx";
        //var folder = _fileService.GetTemplateFolder();
        //var result = _fileService.GetFileStreamResult(folder, templateName, true);

        IImporter Importer = new ExcelImporter();
        var byteArray = await Importer.GenerateTemplateBytes<GenTestImportInput>();
        var result = _fileService.GetFileStreamResult(byteArray, templateName);

        //IExcelExporter exporter = new ExcelExporter();
        //var byteArray = await exporter.ExportHeaderAsByteArray(new GenTestExport());
        //var result = new FileStreamResult(new MemoryStream(byteArray), "application/octet-stream") { FileDownloadName = "学生信息.xlsx" };
        return result;

    }


    /// <inheritdoc/>
    public async Task<dynamic> Preview(BaseImportPreviewInput input)
    {
        _fileService.ImportVerification(input.File);
        IImporter Importer = new ExcelImporter();
        using var fileStream = input.File.OpenReadStream();//获取文件流
        var import = await Importer.Import<GenTestImportInput>(fileStream);//导入的文件转化为带入结果
        var ImportPreview = _fileService.TemplateDataVerification(import);//验证数据完整度
        //遍历错误的行
        //import.RowErrors.ForEach(row =>
        //{
        //    row.RowIndex -= 2;//下表与列表中的下标一致
        //    ImportPreview.Data[row.RowIndex].HasError = true;//错误的行HasError = true
        //    ImportPreview.Data[row.RowIndex].ErrorInfo = row.FieldErrors;
        //});
        ImportPreview.Data = await CheckImport(ImportPreview.Data);
        return ImportPreview;
    }


    /// <inheritdoc/>
    public async Task<BaseImportResultOutPut<GenTestImportInput>> Import(BaseImportResultInput<GenTestImportInput> input)
    {

        var data = await CheckImport(input.Data, true);
        var result = new BaseImportResultOutPut<GenTestImportInput> { Total = input.Data.Count };
        var importData = data.Where(it => it.HasError == false).ToList();
        if (importData.Count != data.Count)
        {
            result.Success = false;
            result.Data = data.Where(it => it.HasError == true).ToList();
            result.FailCount = data.Count - importData.Count;
        }
        result.ImportCount = importData.Count;
        var genTests = importData.Adapt<List<GenTest>>();//转实体
        //await InsertRangeAsync(genTests);//导入用户
        //DbContext.Db.Fastest<GenTest>().BulkCopy(genTests);//性能 比现有任何Bulkcopy都要快30%
        Thread.Sleep(1111);
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
        IExporter exporter = new ExcelExporter();
        var byteArray = await exporter.ExportAsByteArray(data);
        var result = _fileService.GetFileStreamResult(byteArray, "学生信息.xlsx");
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


