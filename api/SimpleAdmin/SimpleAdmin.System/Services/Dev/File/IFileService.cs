using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 文件下载
    /// </summary>
    /// <param name="input">文件iD</param>
    /// <returns>文件流</returns>
    Task<FileStreamResult> Download(BaseIdInput input);

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
    Task UploadFile(string engine, IFormFile file);
}
