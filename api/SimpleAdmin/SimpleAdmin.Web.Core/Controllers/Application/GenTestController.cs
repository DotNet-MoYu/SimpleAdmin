using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Furion.DynamicApiController;
using SimpleAdmin.Application;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 测试控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "测试")]
[Route("/biz/test")]
public class GenTestController : IDynamicApiController
{
    private readonly IGenTestService _genTestService;

    public GenTestController(IGenTestService genTestService
    )
    {
        _genTestService = genTestService;
    }

    #region Get请求

    /// <summary>
    /// 测试分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [DisplayName("测试分页查询")]
    public async Task<dynamic> Page([FromQuery] GenTestPageInput input)
    {
        return await _genTestService.Page(input);
    }

    /// <summary>
    /// 测试列表查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("list")]
    [DisplayName("测试列表查询")]
    public async Task<dynamic> List([FromQuery] GenTestPageInput input)
    {
        return await _genTestService.List(input);
    }

    /// <summary>
    /// 测试详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [DisplayName("测试详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _genTestService.Detail(input);
    }

    /// <summary>
    /// 测试导入预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    public async Task<dynamic> Preview([FromForm] ImportPreviewInput input)
    {
        return await _genTestService.Preview(input);
    }

    /// <summary>
    /// 测试导入模板下载
    /// </summary>
    /// <returns></returns>
    [HttpGet("template")]
    [SuppressMonitor]
    public async Task<dynamic> Template()
    {
        return await _genTestService.Template();
    }

    #endregion


    #region Post请求

    /// <summary>
    /// 添加测试
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加测试")]
    public async Task Add([FromBody] GenTestAddInput input)
    {
        await _genTestService.Add(input);
    }

    /// <summary>
    /// 修改测试
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改测试")]
    public async Task Edit([FromBody] GenTestEditInput input)
    {
        await _genTestService.Edit(input);
    }

    /// <summary>
    /// 删除测试
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除测试")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _genTestService.Delete(input);
    }

    /// <summary>
    /// 测试导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [DisplayName("测试导入")]
    public async Task<dynamic> Import([SuppressMonitor][FromBody] ImportResultInput<GenTestImportInput> input)
    {
        return await _genTestService.Import(input);
    }

    /// <summary>
    /// 测试导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("export")]
    [DisplayName("测试导出")]
    public async Task<dynamic> Export([FromBody] GenTestPageInput input)
    {
        return await _genTestService.Export(input);
    }

    #endregion
}