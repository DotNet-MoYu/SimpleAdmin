using Furion.Extensions;
using Magicodes.ExporterAndImporter.Core.Models;
using Microsoft.AspNetCore.Mvc;
using NewLife;
using System.DrawingCore;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace SimpleAdmin.System
{
    /// <summary>
    /// <inheritdoc cref="IFileService"/>
    /// </summary>
    public class FileService : DbRepository<DevFile>, IFileService
    {
        private readonly IConfigService _configService;

        public FileService(IConfigService configService)
        {
            this._configService = configService;
        }



        /// <inheritdoc/>
        public async Task<SqlSugarPagedList<DevFile>> Page(FilePageInput input)
        {
            var query = Context.Queryable<DevFile>()
                             .WhereIF(!string.IsNullOrEmpty(input.Engine), it => it.Engine == input.Engine)//根据关键字查询
                             .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                             .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                             .OrderBy(it => it.Id);
            var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
            return pageInfo;
        }


        /// <inheritdoc/>
        public async Task UploadFile(string engine, IFormFile file)
        {
            await StorageFile(engine, file);

        }

        /// <inheritdoc/>
        public async Task Delete(List<BaseIdInput> input)
        {
            var ids = input.Select(it => it.Id).ToList();//获取ID
            await DeleteByIdsAsync(ids.Cast<object>().ToArray());//根据ID删除数据库

        }

        /// <inheritdoc/>
        public FileStreamResult GetFileStreamResult(string path, string fileName, bool isPathFolder = false)
        {
            if (isPathFolder) path = path.CombinePath(fileName);
            fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
            //文件转流
            var result = new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
            return result;
        }

        /// <inheritdoc/>
        public FileStreamResult GetFileStreamResult(byte[] byteArray, string fileName)
        {
            fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
            //文件转流
            var result = new FileStreamResult(new MemoryStream(byteArray), "application/octet-stream") { FileDownloadName = fileName };
            return result;
        }

        /// <inheritdoc/>
        public async Task<FileStreamResult> Download(BaseIdInput input)
        {
            var devFile = await GetByIdAsync(input.Id);
            if (devFile != null)
            {
                if (devFile.Engine != DevDictConst.FILE_ENGINE_LOCAL)
                    throw Oops.Bah($"非本地文件不支持此方式下载");
                var result = GetFileStreamResult(devFile.StoragePath, devFile.Name);
                return result;
            }
            else
            {
                return null;
            }

        }

        /// <inheritdoc/>
        public void ImportVerification(IFormFile file, int maxSzie = 30, string[] allowTypes = null)
        {

            if (file == null) throw Oops.Bah("文件不能为空");
            if (file.Length > maxSzie * 1024 * 1024) throw Oops.Bah($"文件大小不允许超过{maxSzie}M");
            var fileSuffix = Path.GetExtension(file.FileName).ToLower().Split(".")[1]; // 文件后缀
            string[] allowTypeS = allowTypes == null ? new string[] { "xlsx" } : allowTypes;//允许上传的文件类型
            if (!allowTypeS.Contains(fileSuffix)) throw Oops.Bah(errorMessage: "文件格式错误");

        }

        /// <inheritdoc/>
        public BaseImportPreviewOutput<T> TemplateDataVerification<T>(ImportResult<T> importResult, int maxRowsCount = 0) where T : BaseImportTemplateInput
        {
            if (importResult.Data == null)
                throw Oops.Bah("文件数据格式有误,请重新导入!");
            if (importResult.Exception != null) throw Oops.Bah("导入异常,请检查文件格式!");
            if (maxRowsCount > 0 && importResult.Data.Count > maxRowsCount) throw Oops.Bah($"单次导入数量为{maxRowsCount},请分批次导入!");
            ////遍历模板错误
            importResult.TemplateErrors.ForEach(error =>
            {
                if (error.Message.Contains("not found")) throw Oops.Bah($"列[{error.RequireColumnName}]未找到");
                else throw Oops.Bah($"列[{error.RequireColumnName}]:{error.Message}");

            });

            //导入结果输出
            var importPreview = new BaseImportPreviewOutput<T>() { HasError = importResult.HasError };
            Dictionary<string, string> headerMap = new Dictionary<string, string>();
            //遍历导入的表头列表信息
            importResult.ImporterHeaderInfos.ForEach(it =>
            {
                headerMap.Add(it.Header.Name, it.PropertyName);
                var tableColumns = new TableColumns { Title = it.Header.Name.Split("(")[0], DataIndex = it.PropertyName.FirstCharToLower() };//定义表头,部分表头有说明用(分组去掉说明
                var antTableAttribute = it.PropertyInfo.GetCustomAttribute<AntTableAttribute>();//获取表格特性
                if (antTableAttribute != null)
                {
                    tableColumns.Date = antTableAttribute.IsDate;
                    tableColumns.Ellipsis = antTableAttribute.Ellipsis;
                    tableColumns.Width = antTableAttribute.Width;
                }
                importPreview.TableColumns.Add(tableColumns);//添加到表头
            });

            //导入的数据转集合
            var data = importResult.Data.ToList();
            var systemError = new string[] { };//系统错误提示
            //遍历错误列,将错误字典中的中文改成英文跟实体对应
            importResult.RowErrors.ForEach(row =>
            {
                IDictionary<string, string> fieldErrors = new Dictionary<string, string>();//定义字典
                //遍历错误列,赋值给新的字典
                row.FieldErrors.ForEach(it =>
                {
                    var errrVaule = it.Value;
                    //value xx Invalid, please fill in the correct integer value!
                    //value xx Invalid, please fill in the correct date and time format!
                    if (it.Value.Contains("Invalid"))//如果错误信息有Invalid就提示格式错误
                        errrVaule = $"{it.Key}格式错误";
                    fieldErrors.Add(headerMap[it.Key], errrVaule);
                });
                row.FieldErrors = fieldErrors;//替换新的字典
                row.RowIndex -= 2;//下表与列表中的下标一致
                data[row.RowIndex].HasError = true;//错误的行HasError = true
                data[row.RowIndex].ErrorInfo = fieldErrors;//替换新的字典
            });
            data = data.OrderByDescending(it => it.HasError).ToList();//排序
            importPreview.Data = data;//重新赋值data
            return importPreview;

        }



        /// <inheritdoc/>
        public string GetTemplateFolder()
        {
            var folder = App.WebHostEnvironment.WebRootPath.CombinePath("Template");
            return folder;
        }



        #region 方法
        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="file"></param>
        private async Task StorageFile(string engine, IFormFile file)
        {

            string bucketName = string.Empty;    // 存储桶名称
            string storageUrl = string.Empty;// 定义存储的url，本地文件返回文件实际路径，其他引擎返回网络地址
            var objectId = YitIdHelper.NextId();//生成id

            switch (engine)
            {
                //存储本地
                case DevDictConst.FILE_ENGINE_LOCAL:
                    bucketName = "defaultBucketName";// 存储桶名称
                    storageUrl = await StorageLocal(objectId, file);
                    break;
                //存储本地
                case DevDictConst.FILE_ENGINE_MINIO:
                    var config = await _configService.GetByConfigKey(CateGoryConst.Config_FILE_MINIO, DevConfigConst.FILE_MINIO_DEFAULT_BUCKET_NAME);
                    if (config != null)
                    {
                        bucketName = config.ConfigValue;// 存储桶名称
                        storageUrl = await StorageMinio(objectId, file);
                    }
                    break;
                default:

                    throw Oops.Bah($"不支持的文件引擎");
            }
            var fileSizeKb = (long)(file.Length / 1024.0); // 文件大小KB
            var fileSuffix = Path.GetExtension(file.FileName).ToLower(); // 文件后缀
            DevFile devFile = new DevFile
            {
                Id = objectId,
                Engine = engine,
                Bucket = bucketName,
                Name = file.FileName,
                Suffix = fileSuffix.Split(".")[1],
                ObjName = $"{objectId}{fileSuffix}",
                SizeKb = fileSizeKb,
                SizeInfo = GetSizeInfo(fileSizeKb),
                StoragePath = storageUrl,

            };
            if (engine != CateGoryConst.Config_FILE_LOCAL)//如果不是本地，设置下载地址
            {
                devFile.DownloadPath = storageUrl;
            }
            //如果是图片,生成缩略图
            if (IsPic(fileSuffix))
            {

                //$"data:image/png;base64," + imgByte;
                using var fileStream = file.OpenReadStream();//获取文件流
                var image = Image.FromStream(fileStream);//获取图片
                var thubnail = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);//压缩图片
                var thubnailBase64 = ImageUtil.ImgToBase64String(thubnail);//转base64
                devFile.Thumbnail = $"data:image/png;base64," + thubnailBase64;
            }
            await InsertAsync(devFile);

        }

        /// <summary>
        /// 存储本地文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="file"></param>
        private async Task<string> StorageLocal(long fileId, IFormFile file)
        {
            string uploadFileFolder;
            string configKey = string.Empty;
            //判断是windos还是linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configKey = DevConfigConst.FILE_LOCAL_FOLDER_FOR_UNIX; //Linux

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configKey = DevConfigConst.FILE_LOCAL_FOLDER_FOR_WINDOWS;  //Windows
            }
            //获取路径配置
            var config = await _configService.GetByConfigKey(CateGoryConst.Config_FILE_LOCAL, configKey);
            if (config != null)
            {
                uploadFileFolder = config.ConfigValue;//赋值路径
                var now = DateTime.Now.ToString("d");
                var filePath = Path.Combine(uploadFileFolder, now);
                if (!Directory.Exists(filePath))//如果不存在就创建文件夹
                    Directory.CreateDirectory(filePath);
                var fileSuffix = Path.GetExtension(file.FileName).ToLower(); // 文件后缀
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

        private async Task<string> StorageMinio(long fileId, IFormFile file)
        {
            var minioService = App.GetService<MinioUtils>();
            var now = DateTime.Now.ToString("d");
            var fileSuffix = Path.GetExtension(file.FileName).ToLower(); // 文件后缀
            var fileObjectName = $"{now}/{fileId}{fileSuffix}";//存储后的文件名
            return await minioService.PutObjectAsync(fileObjectName, file);


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
                ".png", ".bmp", ".gif", ".jpg", ".jpeg",".psd"
            };
            if (pics.Contains(suffix))
                return true;
            else
                return false;
        }

        #endregion
    }
}
