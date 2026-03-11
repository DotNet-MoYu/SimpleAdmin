namespace SimpleAdmin.Web.Core;

[ApiDescriptionSettings("Application", Tag = "文件日志")]
[Route("biz/document/log")]
[RolePermission]
public class DocumentLogController : IDynamicApiController
{
    private readonly IDocumentLogService _documentLogService;

    public DocumentLogController(IDocumentLogService documentLogService)
    {
        _documentLogService = documentLogService;
    }

    [HttpGet("page")]
    [DisplayName("文件日志分页查询")]
    public async Task<dynamic> Page([FromQuery] DocumentLogPageInput input) => await _documentLogService.Page(input);

    [HttpPost("empty")]
    [DisplayName("清空文件日志")]
    public async Task Empty() => await _documentLogService.Empty();
}
