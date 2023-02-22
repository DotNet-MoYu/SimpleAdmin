using Magicodes.ExporterAndImporter.Core.Models;
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
    /// 获取导入模板文件夹路径
    /// </summary>
    /// <returns>文件夹</returns>
    string GetTemplateFolder();

    /// <summary>
    /// 验证上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="maxSzie">最大体积(M)</param>
    /// <param name="allowTypes">允许上传类型</param>
    void ImportVerification(IFormFile file, int maxSzie = 30, string[] allowTypes = null);

    /// <summary>
    /// 文件分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>文件列表</returns>
    Task<SqlSugarPagedList<DevFile>> Page(FilePageInput input);

    /// <summary>
    /// 模板数据验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="importResult">导入结果</param>
    /// <param name="maxRowsCount"></param>
    ImportPreviewOutput<T> TemplateDataVerification<T>(ImportResult<T> importResult, int maxRowsCount = 0) where T : class;

    /// <summary>
    /// 上传文件到本地返回下载url
    /// </summary>
    /// <param name="engine">文件引擎</param>
    /// <param name="file">文件</param>
    /// <returns></returns>
    Task UploadFile(string engine, IFormFile file);
}
