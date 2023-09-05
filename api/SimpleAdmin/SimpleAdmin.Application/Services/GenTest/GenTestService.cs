// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Microsoft.Extensions.Logging;

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

    #endregion 查询

    #region 新增

    /// <inheritdoc />
    public async Task Add(GenTestAddInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await InsertAsync(genTest);//插入数据
    }

    #endregion 新增

    #region 编辑

    /// <inheritdoc />
    public async Task Edit(GenTestEditInput input)
    {
        var genTest = input.Adapt<GenTest>();//实体转换
        await CheckInput(genTest);//检查参数
        await UpdateAsync(genTest);//更新数据
    }

    #endregion 编辑

    #region 删除

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
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

    #endregion 删除

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

    #endregion 导入导出

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="genTest"></param>
    private async Task CheckInput(GenTest genTest)
    {
        var errorMessage = $"您没有权限操作该数据";
        if (genTest.Id == SimpleAdminConst.ZERO)
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

    #endregion 方法
}
