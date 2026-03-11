namespace SimpleAdmin.Web.Core;

[ApiDescriptionSettings("Application", Tag = "文件管理")]
[Route("biz/document")]
[RolePermission]
public class DocumentController : IDynamicApiController
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet("page")]
    [DisplayName("文件分页查询")]
    public async Task<dynamic> Page([FromQuery] DocumentPageInput input) => await _documentService.Page(input);

    [HttpGet("tree")]
    [DisplayName("文件夹树")]
    public async Task<dynamic> Tree() => await _documentService.Tree();

    [HttpGet("detail")]
    [DisplayName("文件详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input) => await _documentService.Detail(input);

    [HttpPost("addFolder")]
    [DisplayName("新建文件夹")]
    public async Task<long> AddFolder([FromBody] AddFolderInput input) => await _documentService.AddFolder(input);

    [HttpPost("rename")]
    [DisplayName("重命名文件")]
    public async Task Rename([FromBody] RenameDocumentInput input) => await _documentService.Rename(input);

    [HttpPost("move")]
    [DisplayName("移动文件")]
    public async Task Move([FromBody] MoveDocumentInput input) => await _documentService.Move(input);

    [HttpPost("delete")]
    [DisplayName("删除文件")]
    public async Task Delete([FromBody] BaseIdInput input) =>
        await _documentService.Delete(new BaseIdListInput { Ids = new List<long> { input.Id } });

    [HttpPost("batchDelete")]
    [DisplayName("批量删除文件")]
    public async Task BatchDelete([FromBody] BaseIdListInput input) => await _documentService.Delete(input);

    [HttpPost("uploadFiles")]
    [DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    [DisplayName("上传文件")]
    public async Task UploadFiles([FromForm] UploadDocumentInput input) => await _documentService.UploadFiles(input);

    [HttpPost("uploadFolder")]
    [DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    [DisplayName("上传文件夹")]
    public async Task UploadFolder([FromForm] UploadDocumentInput input) => await _documentService.UploadFolder(input);

    [HttpGet("download")]
    [DisplayName("下载文件")]
    public async Task<IActionResult> Download([FromQuery] BaseIdInput input) => await _documentService.Download(input);

    [HttpGet("preview")]
    [DisplayName("预览文件")]
    public async Task<dynamic> Preview([FromQuery] BaseIdInput input) => await _documentService.Preview(input);

    [HttpGet("grantDetail")]
    [SuperAdmin]
    [DisplayName("文件夹授权详情")]
    public async Task<dynamic> GrantDetail([FromQuery] BaseIdInput input) => await _documentService.GrantDetail(input);

    [HttpPost("grantUsers")]
    [SuperAdmin]
    [DisplayName("文件夹授权用户")]
    public async Task GrantUsers([FromBody] GrantDocumentUsersInput input) => await _documentService.GrantUsers(input);

    [HttpPost("grantRoles")]
    [SuperAdmin]
    [DisplayName("文件夹授权角色")]
    public async Task GrantRoles([FromBody] GrantDocumentRolesInput input) => await _documentService.GrantRoles(input);
}
