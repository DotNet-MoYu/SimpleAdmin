namespace SimpleAdmin.Web.Core;

/// <summary>
/// 代码生成配置控制器
/// </summary>
[ApiDescriptionSettings(Tag = "代码生成配置")]
[Route("sys/curd/gen/config")]
[SuperAdmin]
public class GenConfigController : IDynamicApiController
{
    private readonly IGenConfigService _genConfigService;

    public GenConfigController(IGenConfigService genConfigService)
    {
        _genConfigService = genConfigService;
    }

    /// <summary>
    /// 查询代码生成详细配置列表
    /// </summary>
    /// <param name="basicId"></param>
    /// <returns></returns>
    [HttpGet("list")]
    [QueryParameters]
    public async Task<dynamic> List(string basicId)
    {
        return await _genConfigService.List(basicId.ToLong());
    }

    /// <summary>
    /// 编辑代码生成详细
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("编辑代码生成详细")]
    [HttpPost("editBatch")]
    public async Task EditBatch([FromBody] List<GenConfig> input)
    {
        await _genConfigService.EditBatch(input);
    }
}
