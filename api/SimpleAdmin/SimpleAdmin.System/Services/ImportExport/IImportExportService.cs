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
    /// <param name="maxSize">文件最大体积(M)</param>
    /// <param name="allowTypes">允许的格式</param>
    void ImportVerification(IFormFile file, int maxSize = 30, string[] allowTypes = null);

    /// <summary>
    /// 模板数据验证
    /// </summary>
    /// <typeparam name="T">模板类实体</typeparam>
    /// <param name="importResult">结果</param>
    /// <returns>导入预览数据</returns>
    ImportPreviewOutput<T> TemplateDataVerification<T>(ImportResult<T> importResult) where T : ImportTemplateInput;
}
