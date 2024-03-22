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
///  <inheritdoc cref="IImportExportService"/>
/// </summary>
public class ImportExportService : IImportExportService
{
    #region 导入

    /// <inheritdoc/>
    public void ImportVerification(IFormFile file, int maxSize = 30, string[] allowTypes = null)
    {
        if (file == null) throw Oops.Bah("文件不能为空");
        if (file.Length > maxSize * 1024 * 1024) throw Oops.Bah($"文件大小不允许超过{maxSize}M");
        var fileSuffix = Path.GetExtension(file.FileName).ToLower().Split(".")[1];// 文件后缀
        var allowTypeS = allowTypes == null ? new[] { "xlsx" } : allowTypes;//允许上传的文件类型
        if (!allowTypeS.Contains(fileSuffix)) throw Oops.Bah(errorMessage: "文件格式错误");
    }

    /// <inheritdoc/>
    public ImportPreviewOutput<T> TemplateDataVerification<T>(ImportResult<T> importResult) where T : ImportTemplateInput
    {
        if (importResult.Exception != null) throw Oops.Bah("导入异常,请检查文件格式!");
        ////遍历模板错误
        importResult.TemplateErrors.ForEach(error =>
        {
            if (error.Message.Contains("not found")) throw Oops.Bah($"列[{error.RequireColumnName}]未找到");
            throw Oops.Bah($"列[{error.RequireColumnName}]:{error.Message}");
        });
        if (importResult.Data == null)
            throw Oops.Bah("文件数据格式有误,请重新导入!");

        //导入结果输出
        var importPreview = new ImportPreviewOutput<T> { HasError = importResult.HasError };
        var headerMap = new Dictionary<string, string>();
        //遍历导入的表头列表信息
        importResult.ImporterHeaderInfos.ForEach(it =>
        {
            headerMap.Add(it.Header.Name, it.PropertyName);
            var tableColumns = new TableColumns
                { Title = it.Header.Name.Split("(")[0], DataIndex = it.PropertyName.FirstCharToLower() };//定义表头,部分表头有说明用(分组去掉说明
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
    public async Task<FileStreamResult> GenerateTemplate<T>(string fileName) where T : class, new()
    {
        IImporter importer = new ExcelImporter();
        var byteArray = await importer.GenerateTemplateBytes<T>();
        var result = GetFileStreamResult(byteArray, fileName);
        return result;
    }

    /// <inheritdoc/>
    public FileStreamResult GenerateLocalTemplate(string fileName, string templateFolder = "Template")
    {
        var folder = App.WebHostEnvironment.WebRootPath.CombinePath(templateFolder);
        return GetFileStreamResult(folder, fileName, true);
    }

    /// <inheritdoc/>
    public async Task<ImportPreviewOutput<T>> GetImportPreview<T>(IFormFile file) where T : ImportTemplateInput, new()
    {
        ImportVerification(file);//验证文件
        IImporter importer = new ExcelImporter();
        using var fileStream = file.OpenReadStream();//获取文件流
        var import = await importer.Import<T>(fileStream);//导入的文件转化为带入结果
        var importPreview = TemplateDataVerification(import);//验证数据完整度
        return importPreview;
    }

    /// <inheritdoc/>
    public ImportResultOutPut<T> GetImportResultPreview<T>(List<T> data, out List<T> importData) where T : ImportTemplateInput
    {
        //定义结果
        var result = new ImportResultOutPut<T> { Total = data.Count };
        //可以导入的数据
        importData = data.Where(it => it.HasError == false).ToList();
        //如果有错误
        if (importData.Count != data.Count)
        {
            result.Success = false;
            result.Data = data.Where(it => it.HasError).ToList();
            result.FailCount = data.Count - importData.Count;
        }
        result.ImportCount = importData.Count;
        return result;
    }

    #endregion 导入

    #region 导出

    public async Task<FileStreamResult> Export<T>(List<T> data, string fileName) where T : class, new()
    {
        IExporter exporter = new ExcelExporter();
        var byteArray = await exporter.ExportAsByteArray(data);
        var result = GetFileStreamResult(byteArray, fileName);
        return result;
    }

    #endregion 导出

    #region 方法

    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="isPathFolder"></param>
    /// <returns></returns>
    public FileStreamResult GetFileStreamResult(string path, string fileName, bool isPathFolder = false)
    {
        fileName = GetFileName(fileName);
        if (isPathFolder) path = path.CombinePath(fileName);
        //文件转流
        var result = new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
        return result;
    }

    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="byteArray"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public FileStreamResult GetFileStreamResult(byte[] byteArray, string fileName)
    {
        fileName = GetFileName(fileName);
        //文件转流
        var result = new FileStreamResult(new MemoryStream(byteArray), "application/octet-stream") { FileDownloadName = fileName };
        return result;
    }

    /// <summary>
    /// 获取文件名
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetFileName(string fileName)
    {
        if (!fileName.Contains("."))
            fileName = fileName + ".xlsx";
        fileName = HttpUtility.UrlEncode(fileName, Encoding.GetEncoding("UTF-8"));//文件名转utf8不然前端下载会乱码
        return fileName;
    }

    /// <summary>
    /// 获取本地模板路径
    /// </summary>
    /// <returns></returns>
    public string GetTemplateFolder()
    {
        var folder = App.WebHostEnvironment.WebRootPath.CombinePath("Template");
        return folder;
    }

    #endregion 方法
}
