// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 测试控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "测试")]
[Route("/biz/test")]
public class GenTestController : IDynamicApiController
{
    private readonly IGenTestService _genTestService;
    private readonly ISysOrgService _sysOrgService;

    public GenTestController(IGenTestService genTestService
        , ISysOrgService sysOrgService
    )
    {
        _genTestService = genTestService;
        _sysOrgService = sysOrgService;
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
    /// 获取组织树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    [DisplayName("组织树查询")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _sysOrgService.Tree();
    }

    /// <summary>
    /// 测试导入预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    [DisplayName("测试导入预览")]
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

    #endregion Get请求

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

    /// <summary>
    /// 测试导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [DisplayName("测试导入")]
    public async Task<dynamic> Import(
        [SuppressMonitor][FromBody] ImportResultInput<GenTestImportInput> input)
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

    #endregion Post请求
}