using Furion.FriendlyException;
using Mapster;
using SimpleAdmin.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace SimpleAdmin.Application;

/// <summary>
/// <inheritdoc cref="IGenTestService"/>
/// </summary>
public class GenTestService : DbRepository<GenTest>, IGenTestService
{
    private readonly ILogger<GenTestService> _logger;
    private readonly ISysUserService _sysUserService;
    private readonly IImportExportService _importExportService;
    private readonly ISysOrgService _sysOrgService;

    public GenTestService(ILogger<GenTestService> logger, ISysUserService sysUserService
        , IImportExportService importExportService
        , ISysOrgService sysOrgService
    )
    {
        _sysUserService = sysUserService;
        _logger = logger;
        _importExportService = importExportService;
        _sysOrgService = sysOrgService;
    }

    #region 查询

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<GenTest>> Page(GenTestPageInput input)
    {
        var query = await GetQuery(input);//获取查询条件
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<List<GenTest>> List(GenTestPageInput input)
    {
        var query = await GetQuery(input);//获取查询条件
        var list = await query.ToListAsync();
        return list;
    }

    /// <inheritdoc />
    public async Task<GenTest> Detail(BaseIdInput input)
    {
        var genTest = await GetFirstAsync(it => it.Id == input.Id);
        return genTest;
    }

    //外键查询

    #endregion

    #region 新增

    /// <inheritdoc />
    public async Task Add(GenTestAddInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await InsertAsync(genTest);//插入数据
    }

    #endregion

    #region 编辑

    /// <inheritdoc />
    public async Task Edit(GenTestEditInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await UpdateAsync(genTest);//更新数据
    }

    #endregion

    #region 删除

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除数据
            ////事务
            //var result = await itenant.UseTranAsync(async () =>
            //{
            //await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除数据
            //});
            //if (!result.IsSuccess)//如果成功了
            //{
            ////写日志
            //_logger.LogError(result.ErrorMessage, result.ErrorException);
            //throw Oops.Oh(ErrorCodeEnum.A0002);
            //} 
        }
    }

    #endregion

    #region 导入导出

    /// <inheritdoc/>
    public async Task<FileStreamResult> Template()
    {
        var templateName = "测试信息";
        //var result = _importExportService.GenerateLocalTemplate(templateName);
        var result = await _importExportService.GenerateTemplate<GenTestImportInput>(templateName);
        return result;
    }

    /// <inheritdoc/>
    public async Task<ImportPreviewOutput<GenTestImportInput>> Preview(ImportPreviewInput input)
    {
        var importPreview =
            await _importExportService.GetImportPreview<GenTestImportInput>(input.File);
        importPreview.Data = await CheckImport(importPreview.Data);//检查导入数据
        return importPreview;
    }

    /// <inheritdoc/>
    public async Task<ImportResultOutPut<GenTestImportInput>> Import(
        ImportResultInput<GenTestImportInput> input)
    {
        var data = await CheckImport(input.Data, true);//检查数据格式
        var result = _importExportService.GetImportResultPreview(data, out var importData);
        var genTest = importData.Adapt<List<GenTest>>();//转实体
        await InsertOrBulkCopy(genTest);// 数据导入
        return result;
    }

    /// <inheritdoc/>
    public async Task<FileStreamResult> Export(GenTestPageInput input)
    {
        var genTests = await List(input);
        var data = genTests.Adapt<List<GenTestExportOutput>>();//转为Dto
        var result = await _importExportService.Export(data, "测试信息");
        return result;
    }

    #endregion

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="genTest"></param>
    private async Task CheckInput(GenTest genTest)
    {
        var errorMessage = $"您没有权限操作该数据";
        if (genTest.Id == SimpleAdminConst.Zero)
        {
            //表示新增
        }
        else
        {
            //表示编辑
        }
    }

    /// <summary>
    /// 获取Sqlsugar的ISugarQueryable
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<ISugarQueryable<GenTest>> GetQuery(GenTestPageInput input)
    {
        var orgIds = await _sysOrgService.GetOrgChildIds(input.OrgId);//获取下级机构
        var query = Context.Queryable<GenTest>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name),
                it => it.Name.Contains(input.Name.Trim()))
            .WhereIF(input.OrgId > 0, it => orgIds.Contains(it.CreateOrgId))//根据机构ID查询
            //.WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField),
                $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode);//排序
        return query;
    }

    /// <inheritdoc/>
    public async Task<List<GenTestImportInput>> CheckImport<GenTestImportInput>(
        List<GenTestImportInput> data, bool clearError = false)
    {
        return data;
    }

    #endregion
}