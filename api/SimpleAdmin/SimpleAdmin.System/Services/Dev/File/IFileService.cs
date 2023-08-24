namespace SimpleAdmin.System;

/// <summary>
/// 文件管理服务
/// </summary>
public interface IFileService : ITransient
{
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="input">ID列表</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 文件下载
    /// </summary>
    /// <param name="input">文件iD</param>
    /// <returns>文件流</returns>
    Task<FileStreamResult> Download(BaseIdInput input);

    /// <summary>
    /// 获取FileStreamResult文件流
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="fileName">文件名</param>
    /// <param name="isPathFolder">路径是否是文件夹</param>
    /// <returns></returns>
    FileStreamResult GetFileStreamResult(string path, string fileName, bool isPathFolder = false);

    /// <summary>
    /// 获取FileStreamResult文件流
    /// </summary>
    /// <param name="byteArray">文件数组</param>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    FileStreamResult GetFileStreamResult(byte[] byteArray, string fileName);

    /// <summary>
    /// 文件分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>文件列表</returns>
    Task<SqlSugarPagedList<DevFile>> Page(FilePageInput input);

    /// <summary>
    /// 上传文件到本地返回下载url
    /// </summary>
    /// <param name="engine">文件引擎</param>
    /// <param name="file">文件</param>
    /// <returns></returns>
    Task<long> UploadFile(string engine, IFormFile file);
}