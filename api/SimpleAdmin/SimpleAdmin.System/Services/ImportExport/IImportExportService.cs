namespace SimpleAdmin.System;

/// <summary>
/// 导入服务
/// </summary>
public interface IImportExportService : ITransient
{
    /// <summary>
    /// 导出数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">数据</param>
    /// <param name="fileName">文件名</param>
    /// <returns>文件流</returns>
    Task<FileStreamResult> Export<T>(List<T> data, string fileName) where T : class, new();

    /// <summary>
    /// 获取本地模板
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="templateFolder">模板文件夹路径:默认wwwroot下的Template</param>
    /// <returns>文件流</returns>
    FileStreamResult GenerateLocalTemplate(string fileName, string templateFolder = "Template");

    /// <summary>
    /// 生成模板
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    /// <param name="fileName">文件名</param>
    /// <returns>文件流</returns>
    Task<FileStreamResult> GenerateTemplate<T>(string fileName) where T : class, new();

    /// <summary>
    /// 获取导入预览
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file">文件</param>
    /// <returns>导入预览结果</returns>
    Task<ImportPreviewOutput<T>> GetImportPreview<T>(IFormFile file) where T : ImportTemplateInput, new();

    /// <summary>
    /// 获取预计导入结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">数据</param>
    /// <param name="importData">成功导入数据</param>
    /// <returns>预计导入结果</returns>
    ImportResultOutPut<T> GetImportResultPreview<T>(List<T> data, out List<T> importData) where T : ImportTemplateInput;

    /// <summary>
    /// 导入文件验证
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="maxSzie">文件最大体积(M)</param>
    /// <param name="allowTypes">允许的格式</param>
    void ImportVerification(IFormFile file, int maxSzie = 30, string[] allowTypes = null);

    /// <summary>
    /// 模板数据验证
    /// </summary>
    /// <typeparam name="T">模板类实体</typeparam>
    /// <param name="importResult">结果</param>
    /// <returns>导入预览数据</returns>
    ImportPreviewOutput<T> TemplateDataVerification<T>(ImportResult<T> importResult) where T : ImportTemplateInput;
}