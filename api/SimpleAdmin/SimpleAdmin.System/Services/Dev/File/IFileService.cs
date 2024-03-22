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
/// 文件管理服务
/// </summary>
public interface IFileService : ITransient
{
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="input">ID列表</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

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
    /// 文件分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>文件列表</returns>
    Task<SqlSugarPagedList<SysFile>> Page(FilePageInput input);

    /// <summary>
    /// 上传文件到本地返回下载url
    /// </summary>
    /// <param name="engine">文件引擎</param>
    /// <param name="file">文件</param>
    /// <returns></returns>
    Task<long> UploadFile(string engine, IFormFile file);
}
