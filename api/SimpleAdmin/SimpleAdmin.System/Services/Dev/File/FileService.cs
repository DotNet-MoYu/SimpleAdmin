using SkiaSharp;
using System.Runtime.InteropServices;
using System.Web;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IFileService"/>
/// </summary>
public class FileService : DbRepository<DevFile>, IFileService
{
    private readonly IConfigService _configService;

    public FileService(IConfigService configService)
    {
        _configService = configService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<DevFile>> Page(FilePageInput input)
    {
        var query = Context.Queryable<DevFile>()
            .WhereIF(!string.IsNullOrEmpty(input.Engine), it => it.Engine == input.Engine)//根据关键字查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.Name.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField),
                $"{input.SortField} {input.SortOrder}")//排序
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
    public async Task Delete(List<BaseIdInput> input)
    {
        var ids = input.Select(it => it.Id).ToList();//获取ID
        await DeleteByIdsAsync(ids.Cast<object>().ToArray());//根据ID删除数据库
    }

    /// <inheritdoc/>
    public FileStreamResult GetFileStreamResult(string path, string fileName,
        bool isPathFolder = false)
    {
        if (isPathFolder) path = path.CombinePath(fileName);
        fileName =
            HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
        //文件转流
        var result =
            new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        return result;
    }

    public async Task<FileStreamResult> GetFileStreamResultFromMinio(string objectName,
        string fileName)
    {
        var minioService = App.GetService<MinioUtils>();
        var stream = await minioService.DownloadFileAsync(objectName);

        fileName =
            HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
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
        fileName =
            HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
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
            if (devFile.Engine == DevDictConst.FILE_ENGINE_LOCAL)
                return GetFileStreamResult(devFile.StoragePath, devFile.Name);
            else if (devFile.Engine == DevDictConst.FILE_ENGINE_MINIO)
                return await GetFileStreamResultFromMinio(devFile.ObjName, devFile.Name);
            else
                return null;
        }
        else
        {
            return null;
        }
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
            case DevDictConst.FILE_ENGINE_LOCAL:
                bucketName = "defaultBucketName";// 存储桶名称
                storageUrl = await StorageLocal(objectId, file);
                break;
            //存储本地
            case DevDictConst.FILE_ENGINE_MINIO:
                var config = await _configService.GetByConfigKey(CateGoryConst.Config_FILE_MINIO,
                    DevConfigConst.FILE_MINIO_DEFAULT_BUCKET_NAME);
                if (config != null)
                {
                    bucketName = config.ConfigValue;// 存储桶名称
                    (objName, storageUrl) = await StorageMinio(objectId, file);
                }
                break;

            default:

                throw Oops.Bah($"不支持的文件引擎");
        }
        var fileSizeKb = (long)(file.Length / 1024.0);// 文件大小KB
        var fileSuffix = Path.GetExtension(file.FileName).ToLower();// 文件后缀
        var devFile = new DevFile
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
        if (engine != CateGoryConst.Config_FILE_LOCAL)//如果不是本地，设置下载地址
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
            devFile.Thumbnail = $"data:image/png;base64," + thubnailBase64;
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
        //判断是windos还是linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            configKey = DevConfigConst.FILE_LOCAL_FOLDER_FOR_UNIX;//Linux
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            configKey = DevConfigConst.FILE_LOCAL_FOLDER_FOR_WINDOWS;//Windows
        }
        //获取路径配置
        var config =
            await _configService.GetByConfigKey(CateGoryConst.Config_FILE_LOCAL, configKey);
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
        else
        {
            throw Oops.Oh($"文件存储路径未配置");
        }
    }

    /// <summary>
    /// 存储到minio
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task<(string objName, string downloadUrl)> StorageMinio(long fileId,
        IFormFile file)
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
        const int MB = 1024 * 1024;
        const int KB = 1024;
        if (b / MB >= 1)
        {
            return Math.Round(b / (float)MB, 2) + "MB";
        }

        if (b / KB >= 1)
        {
            return Math.Round(b / (float)KB, 2) + "KB";
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
        var pics = new string[]
        {
            ".png", ".bmp", ".gif", ".jpg", ".jpeg", ".psd"
        };
        if (pics.Contains(suffix))
            return true;
        else
            return false;
    }

    #endregion 方法
}