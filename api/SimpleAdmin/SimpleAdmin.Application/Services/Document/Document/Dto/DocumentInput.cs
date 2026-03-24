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
using System.ComponentModel.DataAnnotations;

namespace SimpleAdmin.Application;

/// <summary>
/// 文件分页入参
/// </summary>
public class DocumentPageInput : BasePageInput
{
    public long? ParentId { get; set; } = 0;

    public string Name { get; set; }

    public string FileType { get; set; }

    public string Label { get; set; }

    public DocumentNodeType? NodeType { get; set; }

    public string Suffix { get; set; }

    public DateTime? CreateTimeStart { get; set; }

    public DateTime? CreateTimeEnd { get; set; }

    public DateTime? UpdateTimeStart { get; set; }

    public DateTime? UpdateTimeEnd { get; set; }
}

/// <summary>
/// 文件树入参
/// </summary>
public class DocumentTreeInput
{
    public long? ParentId { get; set; }
}

/// <summary>
/// 新建文件夹
/// </summary>
public class AddFolderInput
{
    public long ParentId { get; set; }

    [Required(ErrorMessage = "Name不能为空")]
    public string Name { get; set; }

    public string Label { get; set; }

    public string Remark { get; set; }
}

/// <summary>
/// 重命名
/// </summary>
public class RenameDocumentInput : BaseIdInput
{
    [Required(ErrorMessage = "Name不能为空")]
    public string Name { get; set; }

    public string Label { get; set; }

    public string Remark { get; set; }
}

/// <summary>
/// 移动
/// </summary>
public class MoveDocumentInput
{
    [Required(ErrorMessage = "Ids不能为空")]
    public List<long> Ids { get; set; }

    public long TargetParentId { get; set; }
}

/// <summary>
/// 上传
/// </summary>
public class UploadDocumentInput
{
    public long ParentId { get; set; }

    public string Engine { get; set; }

    public List<string> RelativePaths { get; set; } = new();

    [Required(ErrorMessage = "Files不能为空")]
    public List<IFormFile> Files { get; set; } = new();
}

/// <summary>
/// 初始化分片上传
/// </summary>
public class ChunkUploadInitInput
{
    public long ParentId { get; set; }

    public string Engine { get; set; }

    [Required(ErrorMessage = "FileName不能为空")]
    public string FileName { get; set; }

    [Required(ErrorMessage = "FileSize不能为空")]
    public long FileSize { get; set; }

    [Required(ErrorMessage = "ChunkSize不能为空")]
    public int ChunkSize { get; set; }

    public string FileHash { get; set; }

    public string RelativePath { get; set; }
}

/// <summary>
/// 上传分片
/// </summary>
public class ChunkUploadPartInput
{
    [Required(ErrorMessage = "UploadId不能为空")]
    public long UploadId { get; set; }

    [Required(ErrorMessage = "ChunkIndex不能为空")]
    public int ChunkIndex { get; set; }

    public string ChunkHash { get; set; }

    [Required(ErrorMessage = "Chunk不能为空")]
    public IFormFile Chunk { get; set; }
}

/// <summary>
/// 分片上传状态查询
/// </summary>
public class ChunkUploadStatusInput
{
    [Required(ErrorMessage = "UploadId不能为空")]
    public long UploadId { get; set; }
}

/// <summary>
/// 完成分片上传
/// </summary>
public class ChunkUploadCompleteInput
{
    [Required(ErrorMessage = "UploadId不能为空")]
    public long UploadId { get; set; }
}

/// <summary>
/// 取消分片上传
/// </summary>
public class ChunkUploadCancelInput
{
    [Required(ErrorMessage = "UploadId不能为空")]
    public long UploadId { get; set; }
}

/// <summary>
/// 用户授权
/// </summary>
public class GrantDocumentUsersInput : BaseIdInput
{
    public List<long> UserIds { get; set; } = new();
}

/// <summary>
/// 角色授权
/// </summary>
public class GrantDocumentRolesInput : BaseIdInput
{
    public List<long> RoleIds { get; set; } = new();
}

/// <summary>
/// 文件预览出参
/// </summary>
public class DocumentPreviewOutput
{
    public string PreviewType { get; set; }

    public string ContentType { get; set; }

    public string Content { get; set; }

    public string FileName { get; set; }
}
