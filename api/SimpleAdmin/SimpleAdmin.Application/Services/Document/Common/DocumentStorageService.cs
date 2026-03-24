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
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using SimpleAdmin.Core.Utils;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IDocumentStorageService"/>
public class DocumentStorageService : DbRepository<SysFile>, IDocumentStorageService
{
    private readonly IFileService _fileService;
    private readonly IConfigService _configService;

    public DocumentStorageService(IFileService fileService, IConfigService configService)
    {
        _fileService = fileService;
        _configService = configService;
    }

    public async Task<SysFile> Upload(string engine, IFormFile file)
    {
        var fileId = await _fileService.UploadFile(engine, file);
        var sysFile = await Context.Queryable<SysFile>().Where(it => it.Id == fileId).FirstAsync();
        if (sysFile == null)
            throw Oops.Bah("文件上传失败");
        return sysFile;
    }

    public async Task<BizDocumentUploadSession> FindReusableChunkUpload(long parentId, string engine, string fileName, string relativePath, string fileHash, long fileSize)
    {
        var sessions = await Context.Queryable<BizDocumentUploadSession>()
            .Where(it => !it.IsDelete && it.CreateUserId == UserManager.UserId)
            .Where(it => it.ParentId == parentId && it.Engine == engine && it.FileName == fileName && it.FileSize == fileSize)
            .WhereIF(!string.IsNullOrWhiteSpace(relativePath), it => it.RelativePath == relativePath)
            .WhereIF(string.IsNullOrWhiteSpace(relativePath), it => string.IsNullOrEmpty(it.RelativePath))
            .WhereIF(!string.IsNullOrWhiteSpace(fileHash), it => it.FileHash == fileHash)
            .Where(it => it.UploadStatus != DocumentUploadStatusConst.CANCELLED)
            .OrderByDescending(it => it.Id)
            .ToListAsync();

        foreach (var session in sessions)
        {
            if (session.UploadStatus == DocumentUploadStatusConst.COMPLETED)
                return session;

            if (session.ExpireTime != null && session.ExpireTime < DateTime.Now)
            {
                await ExpireChunkUpload(session);
                continue;
            }

            if (session.UploadStatus == DocumentUploadStatusConst.INIT ||
                session.UploadStatus == DocumentUploadStatusConst.UPLOADING ||
                session.UploadStatus == DocumentUploadStatusConst.FAILED)
            {
                return session;
            }
        }

        return null;
    }

    public async Task<BizDocumentUploadSession> InitChunkUpload(long parentId, long rootId, string engine, string fileName, string relativePath, string fileHash,
        long fileSize, int chunkSize)
    {
        engine = string.IsNullOrWhiteSpace(engine) ? SysDictConst.FILE_ENGINE_LOCAL : engine.Trim().ToUpper();
        if (engine != SysDictConst.FILE_ENGINE_LOCAL)
            throw Oops.Bah("当前仅支持本地引擎分片上传");
        if (chunkSize <= 0)
            throw Oops.Bah("分片大小必须大于0");
        if (fileSize <= 0)
            throw Oops.Bah("文件大小必须大于0");

        var sessionId = CommonUtils.GetSingleId();
        var tempDir = Path.Combine(await GetChunkUploadRoot(), sessionId.ToString());
        Directory.CreateDirectory(GetChunkPartDirectory(tempDir));
        var session = new BizDocumentUploadSession
        {
            Id = sessionId,
            ParentId = parentId,
            RootId = rootId,
            Engine = engine,
            FileName = fileName,
            RelativePath = relativePath,
            FileHash = fileHash,
            FileSize = fileSize,
            ChunkSize = chunkSize,
            ChunkCount = (int)Math.Ceiling(fileSize / (double)chunkSize),
            TempDir = tempDir,
            UploadStatus = DocumentUploadStatusConst.INIT,
            ErrorMessage = string.Empty,
            ExpireTime = DateTime.Now.AddHours(24)
        };
        PrepareCreate(session);
        await Context.Insertable(session).ExecuteCommandAsync();
        return session;
    }

    public async Task<BizDocumentUploadSession> GetChunkUploadSession(long uploadId)
    {
        var session = await Context.Queryable<BizDocumentUploadSession>()
            .Where(it => it.Id == uploadId && !it.IsDelete)
            .FirstAsync();
        if (session == null)
            throw Oops.Bah("上传会话不存在");
        return session;
    }

    public async Task SaveChunk(BizDocumentUploadSession session, int chunkIndex, IFormFile chunk, string chunkHash)
    {
        EnsureChunkCanWrite(session, chunkIndex);
        Directory.CreateDirectory(GetChunkPartDirectory(session.TempDir));
        var targetPath = GetChunkFilePath(session.TempDir, chunkIndex);
        if (File.Exists(targetPath))
        {
            if (string.IsNullOrWhiteSpace(chunkHash))
                return;

            await using var existingStream = File.OpenRead(targetPath);
            var existingHash = await ComputeSha256(existingStream);
            if (existingHash.Equals(chunkHash, StringComparison.OrdinalIgnoreCase))
                return;

            File.Delete(targetPath);
        }

        if (!string.IsNullOrWhiteSpace(chunkHash))
        {
            await using var verifyStream = chunk.OpenReadStream();
            var actualHash = await ComputeSha256(verifyStream);
            if (!actualHash.Equals(chunkHash, StringComparison.OrdinalIgnoreCase))
                throw Oops.Bah("分片校验失败");
        }

        var tempPath = $"{targetPath}.uploading";
        if (File.Exists(tempPath))
            File.Delete(tempPath);
        await using (var stream = File.Create(tempPath))
        {
            await chunk.CopyToAsync(stream);
        }
        if (File.Exists(targetPath))
            File.Delete(targetPath);
        File.Move(tempPath, targetPath);

        if (session.UploadStatus == DocumentUploadStatusConst.INIT || session.UploadStatus == DocumentUploadStatusConst.FAILED)
        {
            session.UploadStatus = DocumentUploadStatusConst.UPLOADING;
            session.ErrorMessage = string.Empty;
            session.ExpireTime = DateTime.Now.AddHours(24);
            PrepareUpdate(session);
            await Context.Updateable(session).UpdateColumns(it => new { it.UploadStatus, it.ErrorMessage, it.ExpireTime, it.UpdateTime, it.UpdateUserId, it.UpdateUser })
                .ExecuteCommandAsync();
        }
    }

    public async Task<List<int>> GetUploadedChunks(BizDocumentUploadSession session)
    {
        await Task.CompletedTask;
        if (session.UploadStatus == DocumentUploadStatusConst.COMPLETED)
            return Enumerable.Range(0, session.ChunkCount).ToList();

        var partDir = GetChunkPartDirectory(session.TempDir);
        if (!Directory.Exists(partDir))
            return new List<int>();

        return Directory.GetFiles(partDir, "*.part", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => int.TryParse(name, out var value) ? value : -1)
            .Where(index => index >= 0)
            .Distinct()
            .OrderBy(index => index)
            .ToList();
    }

    public async Task<SysFile> MergeChunks(BizDocumentUploadSession session)
    {
        var uploadedChunks = await GetUploadedChunks(session);
        if (uploadedChunks.Count != session.ChunkCount)
            throw Oops.Bah("文件分片未上传完成");

        session.UploadStatus = DocumentUploadStatusConst.MERGING;
        session.ErrorMessage = string.Empty;
        PrepareUpdate(session);
        await Context.Updateable(session).UpdateColumns(it => new { it.UploadStatus, it.ErrorMessage, it.UpdateTime, it.UpdateUserId, it.UpdateUser })
            .ExecuteCommandAsync();

        var mergedDir = Path.Combine(session.TempDir, "merged");
        Directory.CreateDirectory(mergedDir);
        var mergedPath = Path.Combine(mergedDir, session.FileName);
        if (File.Exists(mergedPath))
            File.Delete(mergedPath);

        await using (var targetStream = File.Create(mergedPath))
        {
            for (var index = 0; index < session.ChunkCount; index++)
            {
                var chunkPath = GetChunkFilePath(session.TempDir, index);
                if (!File.Exists(chunkPath))
                    throw Oops.Bah($"缺少文件分片：{index}");
                await using var chunkStream = File.OpenRead(chunkPath);
                await chunkStream.CopyToAsync(targetStream);
            }
        }

        var fileId = await _fileService.CreateFileFromPath(new CreateFileFromPathInput
        {
            Engine = session.Engine,
            FileName = session.FileName,
            SourcePath = mergedPath
        });
        var sysFile = await Context.Queryable<SysFile>().Where(it => it.Id == fileId).FirstAsync();
        if (sysFile == null)
            throw Oops.Bah("文件上传失败");
        return sysFile;
    }

    public async Task MarkChunkUploadCompleted(BizDocumentUploadSession session, long fileId, long documentId)
    {
        session.UploadStatus = DocumentUploadStatusConst.COMPLETED;
        session.SysFileId = fileId;
        session.DocumentId = documentId;
        session.ErrorMessage = string.Empty;
        session.ExpireTime = null;
        PrepareUpdate(session);
        await Context.Updateable(session).UpdateColumns(it => new
        {
            it.UploadStatus,
            it.SysFileId,
            it.DocumentId,
            it.ErrorMessage,
            it.ExpireTime,
            it.UpdateTime,
            it.UpdateUserId,
            it.UpdateUser
        }).ExecuteCommandAsync();
        CleanupTempDirectory(session.TempDir);
    }

    public async Task MarkChunkUploadFailed(BizDocumentUploadSession session, string errorMessage)
    {
        session.UploadStatus = DocumentUploadStatusConst.FAILED;
        session.ErrorMessage = errorMessage?.Length > 1000 ? errorMessage[..1000] : errorMessage;
        session.ExpireTime = DateTime.Now.AddHours(24);
        PrepareUpdate(session);
        await Context.Updateable(session).UpdateColumns(it => new { it.UploadStatus, it.ErrorMessage, it.ExpireTime, it.UpdateTime, it.UpdateUserId, it.UpdateUser })
            .ExecuteCommandAsync();
    }

    public async Task CancelChunkUpload(BizDocumentUploadSession session)
    {
        session.UploadStatus = DocumentUploadStatusConst.CANCELLED;
        session.ErrorMessage = string.Empty;
        session.ExpireTime = null;
        PrepareUpdate(session);
        await Context.Updateable(session).UpdateColumns(it => new { it.UploadStatus, it.ErrorMessage, it.ExpireTime, it.UpdateTime, it.UpdateUserId, it.UpdateUser })
            .ExecuteCommandAsync();
        CleanupTempDirectory(session.TempDir);
    }

    public async Task<int> CleanupExpiredChunkUploads(CancellationToken cancellationToken = default)
    {
        var expiredSessions = await Context.Queryable<BizDocumentUploadSession>()
            .Where(it => !it.IsDelete)
            .Where(it => it.ExpireTime != null && it.ExpireTime < DateTime.Now)
            .Where(it => it.UploadStatus != DocumentUploadStatusConst.COMPLETED && it.UploadStatus != DocumentUploadStatusConst.CANCELLED)
            .ToListAsync();
        foreach (var session in expiredSessions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ExpireChunkUpload(session);
        }
        return expiredSessions.Count;
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

    private void EnsureChunkCanWrite(BizDocumentUploadSession session, int chunkIndex)
    {
        if (session.UploadStatus == DocumentUploadStatusConst.COMPLETED)
            return;
        if (session.UploadStatus == DocumentUploadStatusConst.CANCELLED)
            throw Oops.Bah("上传已取消");
        if (session.UploadStatus == DocumentUploadStatusConst.MERGING)
            throw Oops.Bah("文件正在合并中");
        if (chunkIndex < 0 || chunkIndex >= session.ChunkCount)
            throw Oops.Bah("分片索引无效");
    }

    private async Task<string> GetChunkUploadRoot()
    {
        string configKey = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            configKey = SysConfigConst.FILE_LOCAL_FOLDER_FOR_UNIX;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            configKey = SysConfigConst.FILE_LOCAL_FOLDER_FOR_WINDOWS;

        var config = await _configService.GetByConfigKey(CateGoryConst.CONFIG_FILE_LOCAL, configKey);
        if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
            throw Oops.Oh("文件存储路径未配置");

        var root = Path.Combine(config.ConfigValue, "__chunk_upload");
        Directory.CreateDirectory(root);
        return root;
    }

    private string GetChunkPartDirectory(string tempDir)
    {
        return Path.Combine(tempDir, "parts");
    }

    private string GetChunkFilePath(string tempDir, int chunkIndex)
    {
        return Path.Combine(GetChunkPartDirectory(tempDir), $"{chunkIndex}.part");
    }

    private void CleanupTempDirectory(string tempDir)
    {
        if (!string.IsNullOrWhiteSpace(tempDir) && Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
    }

    private async Task ExpireChunkUpload(BizDocumentUploadSession session)
    {
        session.UploadStatus = DocumentUploadStatusConst.CANCELLED;
        session.ErrorMessage = "上传会话已超时清理";
        session.ExpireTime = null;
        PrepareUpdate(session);
        await Context.Updateable(session).UpdateColumns(it => new { it.UploadStatus, it.ErrorMessage, it.ExpireTime, it.UpdateTime, it.UpdateUserId, it.UpdateUser })
            .ExecuteCommandAsync();
        CleanupTempDirectory(session.TempDir);
    }

    private async Task<string> ComputeSha256(Stream stream)
    {
        stream.Position = 0;
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream);
        stream.Position = 0;
        return Convert.ToHexString(hash);
    }

    private void PrepareCreate(BizDocumentUploadSession entity)
    {
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.CreateUserId = UserManager.UserId;
        entity.UpdateUserId = UserManager.UserId;
        entity.CreateUser = UserManager.Name;
        entity.UpdateUser = UserManager.Name;
        entity.CreateOrgId = UserManager.OrgId;
        entity.Status ??= CommonStatusConst.ENABLE;
        entity.IsDelete = false;
    }

    private void PrepareUpdate(BizDocumentUploadSession entity)
    {
        entity.UpdateTime = DateTime.Now;
        entity.UpdateUserId = UserManager.UserId;
        entity.UpdateUser = UserManager.Name;
    }
}
