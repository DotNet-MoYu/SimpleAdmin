// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IFileService"/>
/// </summary>
public class FileService : DbRepository<SysFile>, IFileService
{
    private readonly IConfigService _configService;

    public FileService(IConfigService configService)
    {
        _configService = configService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysFile>> Page(FilePageInput input)
    {
        var query = Context.Queryable<SysFile>().WhereIF(!string.IsNullOrEmpty(input.Engine), it => it.Engine == input.Engine)//根据关键字查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
            .OrderBy(it => it.Id);
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<long> UploadFile(string engine, IFormFile file)
    {
        return await StorageFile(engine, file);
    }

    /// <inheritdoc/>
    public async Task Delete(BaseIdListInput input)
    {
        var ids = input.Ids;//获取ID
        await DeleteByIdsAsync(ids.Cast<object>().ToArray());//根据ID删除数据库
    }

    /// <inheritdoc/>
    public FileStreamResult GetFileStreamResult(string path, string fileName, bool isPathFolder = false)
    {
        if (isPathFolder) path = path.CombinePath(fileName);
        fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
        //文件转流
        var result = new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream")
        {
            FileDownloadName = fileName
        };
        return result;
    }

    public async Task<FileStreamResult> GetFileStreamResultFromMinio(string objectName, string fileName)
    {
        var minioService = App.GetService<MinioUtils>();
        var stream = await minioService.DownloadFileAsync(objectName);

        fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
        //文件转流
        var result = new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileName
        };
        return result;
    }

    /// <inheritdoc/>
    public FileStreamResult GetFileStreamResult(byte[] byteArray, string fileName)
    {
        fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
        //文件转流
        var result = new FileStreamResult(new MemoryStream(byteArray), "application/octet-stream")
        {
            FileDownloadName = fileName
        };
        return result;
    }

    /// <inheritdoc/>
    public async Task<FileStreamResult> Download(BaseIdInput input)
    {
        var devFile = await GetByIdAsync(input.Id);
        if (devFile != null)
        {
            if (devFile.Engine == SysDictConst.FILE_ENGINE_LOCAL)
                return GetFileStreamResult(devFile.StoragePath, devFile.Name);
            if (devFile.Engine == SysDictConst.FILE_ENGINE_MINIO)
                return await GetFileStreamResultFromMinio(devFile.ObjName, devFile.Name);
            return null;
        }
        return null;
    }

    #region 方法

    /// <summary>
    /// 存储文件
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="file"></param>
    private async Task<long> StorageFile(string engine, IFormFile file)
    {
        var bucketName = string.Empty;// 存储桶名称
        var storageUrl = string.Empty;// 定义存储的url，本地文件返回文件实际路径，其他引擎返回网络地址
        var objName = string.Empty;// 定义存储的url，本地文件返回文件实际路径，其他引擎返回网络地址

        var objectId = CommonUtils.GetSingleId();//生成id

        switch (engine)
        {
            //存储本地
            case SysDictConst.FILE_ENGINE_LOCAL:
                bucketName = "defaultBucketName";// 存储桶名称
                storageUrl = await StorageLocal(objectId, file);
                break;
            //存储本地
            case SysDictConst.FILE_ENGINE_MINIO:
                var config = await _configService.GetByConfigKey(CateGoryConst.CONFIG_FILE_MINIO, SysConfigConst.FILE_MINIO_DEFAULT_BUCKET_NAME);
                if (config != null)
                {
                    bucketName = config.ConfigValue;// 存储桶名称
                    (objName, storageUrl) = await StorageMinio(objectId, file);
                }
                break;

            default:

                throw Oops.Bah("不支持的文件引擎");
        }
        var fileSizeKb = (long)(file.Length / 1024.0);// 文件大小KB
        var fileSuffix = Path.GetExtension(file.FileName).ToLower();// 文件后缀
        var devFile = new SysFile
        {
            Id = objectId,
            Engine = engine,
            Bucket = bucketName,
            Name = file.FileName,
            Suffix = fileSuffix.Split(".")[1],
            ObjName = objName,
            SizeKb = fileSizeKb,
            SizeInfo = GetSizeInfo(fileSizeKb),
            StoragePath = storageUrl
        };
        if (engine != CateGoryConst.CONFIG_FILE_LOCAL)//如果不是本地，设置下载地址
        {
            devFile.DownloadPath = storageUrl;
        }
        //如果是图片,生成缩略图
        if (IsPic(fileSuffix))
        {
            //$"data:image/png;base64," + imgByte;
            await using var fileStream = file.OpenReadStream();//获取文件流
            var image = SKImage.FromEncodedData(fileStream);//获取图片
            var bmp = SKBitmap.FromImage(image);
            var thubnail = bmp.GetPicThumbnail(100, 100);//压缩图片
            var thubnailBase64 = ImageUtil.ImgToBase64String(thubnail);//转base64
            devFile.Thumbnail = "data:image/png;base64," + thubnailBase64;
        }
        await InsertAsync(devFile);
        return objectId;
    }

    /// <summary>
    /// 存储本地文件
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="file"></param>
    private async Task<string> StorageLocal(long fileId, IFormFile file)
    {
        string uploadFileFolder;
        var configKey = string.Empty;
        //判断是windows还是linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            configKey = SysConfigConst.FILE_LOCAL_FOLDER_FOR_UNIX;//Linux
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            configKey = SysConfigConst.FILE_LOCAL_FOLDER_FOR_WINDOWS;//Windows
        }
        //获取路径配置
        var config = await _configService.GetByConfigKey(CateGoryConst.CONFIG_FILE_LOCAL, configKey);
        if (config != null)
        {
            uploadFileFolder = config.ConfigValue;//赋值路径
            var now = DateTime.Now.ToString("d");
            var filePath = Path.Combine(uploadFileFolder, now);
            if (!Directory.Exists(filePath))//如果不存在就创建文件夹
                Directory.CreateDirectory(filePath);
            var fileSuffix = Path.GetExtension(file.FileName).ToLower();// 文件后缀
            var fileObjectName = $"{fileId}{fileSuffix}";//存储后的文件名
            var fileName = Path.Combine(filePath, fileObjectName);//获取文件全路局
            fileName = fileName.Replace("\\", "/");//格式化一系
            //存储文件
            using (var stream = File.Create(Path.Combine(filePath, fileObjectName)))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        throw Oops.Oh("文件存储路径未配置");
    }

    /// <summary>
    /// 存储到minio
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task<(string objName, string downloadUrl)> StorageMinio(long fileId, IFormFile file)
    {
        var minioService = App.GetService<MinioUtils>();
        var now = DateTime.Now.ToString("d");
        var fileSuffix = Path.GetExtension(file.FileName).ToLower();// 文件后缀
        var fileObjectName = $"{now}/{fileId}{fileSuffix}";//存储后的文件名
        var downloadUrl = await minioService.PutObjectAsync(fileObjectName, file);
        return (fileObjectName, downloadUrl);
    }

    /// <summary>
    /// 获取文件大小
    /// </summary>
    /// <param name="fileSizeKb"></param>
    /// <returns></returns>
    private string GetSizeInfo(long fileSizeKb)
    {
        var b = fileSizeKb * 1024;
        const int mb = 1024 * 1024;
        const int kb = 1024;
        if (b / mb >= 1)
        {
            return Math.Round(b / (float)mb, 2) + "MB";
        }

        if (b / kb >= 1)
        {
            return Math.Round(b / (float)kb, 2) + "KB";
        }
        if (b == 0)
        {
            return "0B";
        }
        return null;
    }

    /// <summary>
    /// 判断是否是图片
    /// </summary>
    /// <param name="suffix">后缀名</param>
    /// <returns></returns>
    private bool IsPic(string suffix)
    {
        //图片后缀名列表
        var pics = new[]
        {
            ".png", ".bmp", ".gif", ".jpg", ".jpeg", ".psd"
        };
        if (pics.Contains(suffix))
            return true;
        return false;
    }

    #endregion 方法
}
