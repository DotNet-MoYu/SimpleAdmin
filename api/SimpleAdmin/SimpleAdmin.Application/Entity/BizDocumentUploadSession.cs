// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

/// <summary>
/// 分片上传状态
/// </summary>
public class DocumentUploadStatusConst
{
    public const string INIT = "INIT";

    public const string UPLOADING = "UPLOADING";

    public const string MERGING = "MERGING";

    public const string COMPLETED = "COMPLETED";

    public const string CANCELLED = "CANCELLED";

    public const string FAILED = "FAILED";
}

/// <summary>
/// 文件分片上传会话
/// </summary>
[SugarTable("biz_document_upload_session", TableDescription = "业务文件分片上传会话")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
[CodeGen]
public class BizDocumentUploadSession : DataEntityBase
{
    /// <summary>
    /// 上传目标父级ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 授权根目录ID
    /// </summary>
    public long RootId { get; set; }

    /// <summary>
    /// 存储引擎
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Engine { get; set; }

    /// <summary>
    /// 原始文件名
    /// </summary>
    [SugarColumn(Length = 255)]
    public string FileName { get; set; }

    /// <summary>
    /// 相对路径
    /// </summary>
    [SugarColumn(Length = 1000, IsNullable = true)]
    public string RelativePath { get; set; }

    /// <summary>
    /// 文件指纹
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string FileHash { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 分片大小
    /// </summary>
    public int ChunkSize { get; set; }

    /// <summary>
    /// 分片数量
    /// </summary>
    public int ChunkCount { get; set; }

    /// <summary>
    /// 临时目录
    /// </summary>
    [SugarColumn(Length = 1000)]
    public string TempDir { get; set; }

    /// <summary>
    /// 上传状态
    /// </summary>
    [SugarColumn(Length = 50)]
    public string UploadStatus { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 1000, IsNullable = true)]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 对应系统文件ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public long? SysFileId { get; set; }

    /// <summary>
    /// 对应业务文件ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public long? DocumentId { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ExpireTime { get; set; }
}
