namespace SimpleAdmin.Web.Core.Controllers.System.Dev;

/// <summary>
/// 文件管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "文件管理")]
[Route("dev/[controller]")]
public class FileController : BaseController
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        this._fileService = fileService;
    }

    /// <summary>
    /// 文件查询分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] FilePageInput input)
    {
        return await _fileService.Page(input);
    }

    /// <summary>
    /// 上传本地文件
    /// </summary>
    /// <param name="File"></param>
    /// <returns></returns>
    [HttpPost("uploadLocal")]
    [DisplayName("上传本地文件")]
    [DisableRequestSizeLimit]
    public async Task UploadLocal([FromForm] IFormFile File)
    {
        await _fileService.UploadFile(DevDictConst.FILE_ENGINE_LOCAL, File);
    }

    /// <summary>
    /// 上传MINIO文件
    /// </summary>
    /// <param name="File"></param>
    /// <returns></returns>
    [HttpPost("uploadMinio")]
    [DisplayName("上传MINIO文件")]
    [DisableRequestSizeLimit]
    public async Task UploadMinio([FromForm] IFormFile File)
    {
        await _fileService.UploadFile(DevDictConst.FILE_ENGINE_MINIO, File);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除文件")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _fileService.Delete(input);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("download")]
    [DisplayName("下载文件")]
    public async Task<IActionResult> DownLoad([FromQuery] BaseIdInput input)
    {
        return await _fileService.Download(input);
    }
}