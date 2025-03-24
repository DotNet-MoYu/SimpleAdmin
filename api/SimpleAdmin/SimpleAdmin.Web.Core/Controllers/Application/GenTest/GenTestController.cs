using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using MoYu.DynamicApiController;
using SimpleAdmin.Application;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 测试控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "测试")]
[Route("//biz/ops/test")]
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
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _genTestService.Delete(input);
    }

    #endregion
}
