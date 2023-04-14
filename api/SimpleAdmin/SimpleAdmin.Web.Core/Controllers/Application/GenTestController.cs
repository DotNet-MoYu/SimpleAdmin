namespace SimpleAdmin.Web.Core;

/// <summary>
/// 测试控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "测试")]
[Route("/biz/test")]
public class GenTestController : IDynamicApiController
{
    private readonly IGenTestService _genTestService;

    public GenTestController(IGenTestService genTestService)
    {
        this._genTestService = genTestService;
    }

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
    /// 预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    public async Task<dynamic> Preview([FromForm] ImportPreviewInput input)
    {
        return await _genTestService.Preview(input);
    }

    [HttpPost("export")]
    [DisplayName("导出")]
    public async Task<dynamic> Export([FromBody] GenTestPageInput input)
    {
        return await _genTestService.Export(input);
    }

    [HttpGet(template: "template")]
    [DisplayName("模板")]
    public async Task<dynamic> Template()
    {
        return await _genTestService.Template();
    }

    [HttpPost("import")]
    [DisplayName("导入")]
    [SuppressMonitor]
    [LoggingMonitor()]
    public async Task<dynamic> Import([FromBody] ImportResultInput<GenTestImportInput> input)
    {
        return await _genTestService.Import(input);
    }
}