// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Microsoft.AspNetCore.Http;
using System.Text;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IDocumentStorageService"/>
public class DocumentStorageService : DbRepository<SysFile>, IDocumentStorageService
{
    private readonly IFileService _fileService;

    public DocumentStorageService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<SysFile> Upload(string engine, IFormFile file)
    {
        var fileId = await _fileService.UploadFile(engine, file);
        var sysFile = await Context.Queryable<SysFile>().Where(it => it.Id == fileId).FirstAsync();
        if (sysFile == null)
            throw Oops.Bah("文件上传失败");
        return sysFile;
    }

    public async Task<FileStreamResult> Download(long fileId)
    {
        return await _fileService.Download(new BaseIdInput { Id = fileId });
    }

    public async Task<DocumentPreviewOutput> Preview(SysFile file)
    {
        var suffix = file.Suffix?.ToLower() ?? string.Empty;

        // 目前只兜底两类预览：
        // 1. 图片直接返回 base64 / 远程地址；
        // 2. 本地文本文件直接读内容。
        // Office / PDF 暂时返回 none，由前端提示用户下载查看。
        if (new[] { "png", "jpg", "jpeg", "gif", "bmp", "webp" }.Contains(suffix))
        {
            if (file.Engine == SysDictConst.FILE_ENGINE_LOCAL && !string.IsNullOrWhiteSpace(file.StoragePath) && File.Exists(file.StoragePath))
            {
                var bytes = await File.ReadAllBytesAsync(file.StoragePath);
                return new DocumentPreviewOutput
                {
                    PreviewType = "image",
                    ContentType = $"image/{suffix}",
                    FileName = file.Name,
                    Content = $"data:image/{suffix};base64,{Convert.ToBase64String(bytes)}"
                };
            }

            return new DocumentPreviewOutput
            {
                PreviewType = "image",
                ContentType = $"image/{suffix}",
                FileName = file.Name,
                Content = file.DownloadPath
            };
        }

        if (new[] { "txt", "log", "json", "xml", "md", "csv" }.Contains(suffix) && file.Engine == SysDictConst.FILE_ENGINE_LOCAL &&
            !string.IsNullOrWhiteSpace(file.StoragePath) && File.Exists(file.StoragePath))
        {
            return new DocumentPreviewOutput
            {
                PreviewType = "text",
                ContentType = "text/plain",
                FileName = file.Name,
                Content = await File.ReadAllTextAsync(file.StoragePath, Encoding.UTF8)
            };
        }

        return new DocumentPreviewOutput
        {
            PreviewType = "none",
            ContentType = "application/octet-stream",
            FileName = file.Name,
            Content = string.Empty
        };
    }
}
