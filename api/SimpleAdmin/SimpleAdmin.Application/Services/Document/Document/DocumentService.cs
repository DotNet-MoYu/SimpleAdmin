// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using NewLife;
using SimpleAdmin.Core.Utils;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IDocumentService"/>
public class DocumentService : DbRepository<BizDocument>, IDocumentService
{
    private readonly ILogger<DocumentService> _logger;
    private readonly IDocumentAccessService _documentAccessService;
    private readonly IDocumentStorageService _documentStorageService;
    private readonly IDocumentLogService _documentLogService;
    private readonly IRelationService _relationService;
    private readonly ISysUserService _sysUserService;
    private readonly ISysRoleService _sysRoleService;

    public DocumentService(
        ILogger<DocumentService> logger,
        IDocumentAccessService documentAccessService,
        IDocumentStorageService documentStorageService,
        IDocumentLogService documentLogService,
        IRelationService relationService,
        ISysUserService sysUserService,
        ISysRoleService sysRoleService)
    {
        _logger = logger;
        _documentAccessService = documentAccessService;
        _documentStorageService = documentStorageService;
        _documentLogService = documentLogService;
        _relationService = relationService;
        _sysUserService = sysUserService;
        _sysRoleService = sysRoleService;
    }

    public async Task<SqlSugarPagedList<DocumentOutput>> Page(DocumentPageInput input)
    {
        var createTimeEnd = NormalizeRangeEnd(input.CreateTimeEnd);
        var updateTimeEnd = NormalizeRangeEnd(input.UpdateTimeEnd);
        var rootIds = await _documentAccessService.GetAuthorizedRootIds();
        var query = Context.Queryable<BizDocument>()
            .Where(it => it.Visible)
            .Where(it => !it.IsDeleted)
            .WhereIF(rootIds != null && rootIds.Count > 0, it => rootIds.Contains(it.RootId))
            .WhereIF(rootIds is { Count: 0 }, it => it.Id == -1)
            .WhereIF(input.ParentId != null, it => it.ParentId == input.ParentId.Value)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), it => it.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Label), it => it.Label == input.Label)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Suffix), it => it.Suffix == input.Suffix)
            .WhereIF(input.CreateTimeStart != null && createTimeEnd != null, it => SqlFunc.Between(it.CreateTime, input.CreateTimeStart, createTimeEnd))
            .WhereIF(input.UpdateTimeStart != null && updateTimeEnd != null, it => SqlFunc.Between(it.UpdateTime, input.UpdateTimeStart, updateTimeEnd))
            .OrderBy(it => it.NodeType)
            .OrderBy(it => it.Name);
        query = ApplyFileTypeFilter(query, input);
        var outputQuery = query.Select<DocumentOutput>();

        var page = await outputQuery.ToPagedListAsync(input.PageNum, input.PageSize);
        var outputs = await FillDocumentOutputs(page.List.ToList());
        page.List = outputs;
        return page;
    }

    public async Task<List<DocumentTreeOutput>> Tree(DocumentTreeInput input = null)
    {
        var rootIds = await _documentAccessService.GetAuthorizedRootIds();
        var folders = await Context.Queryable<BizDocument>()
            .Where(it => it.Visible && !it.IsDeleted && it.NodeType == DocumentNodeType.Folder)
            .WhereIF(rootIds != null && rootIds.Count > 0, it => rootIds.Contains(it.RootId))
            .WhereIF(rootIds is { Count: 0 }, it => it.Id == -1)
            .OrderBy(it => it.Name)
            .ToTreeAsync(it => it.Children, it => it.ParentId, input?.ParentId ?? 0);

        return folders.Select(MapTree).ToList();
    }

    public async Task<DocumentOutput> Detail(BaseIdInput input)
    {
        var entity = await _documentAccessService.GetDocumentForRead(input.Id);
        var outputs = await FillDocumentOutputs(new List<DocumentOutput> { entity.Adapt<DocumentOutput>() });
        return outputs.First();
    }

    public async Task<long> AddFolder(AddFolderInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw Oops.Bah("文件夹名称不能为空");

        if (input.ParentId == 0 && !UserManager.SuperAdmin)
            throw Oops.Bah("只有超级管理员可以创建授权根目录");

        BizDocument parent = null;
        if (input.ParentId > 0)
        {
            parent = await _documentAccessService.GetDocumentForRead(input.ParentId);
            if (parent.NodeType != DocumentNodeType.Folder)
                throw Oops.Bah("只能在文件夹下创建内容");
        }

        await EnsureUniqueName(input.ParentId, input.Name);
        var entity = CreateFolderEntity(input.ParentId, input.Name, input.Label, input.Remark, parent);
        var folderId = 0L;
        var result = await Tenant.UseTranAsync(async () =>
        {
            PrepareCreate(entity);
            await Context.Insertable(entity).ExecuteCommandAsync();
            folderId = entity.Id;
            if (input.ParentId == 0)
            {
                entity.RootId = entity.Id;
                await Context.Updateable(entity).UpdateColumns(it => new { it.RootId }).ExecuteCommandAsync();
            }
        });
        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("创建文件夹失败");
        }

        await _documentLogService.TryWrite(folderId, "新建文件夹", $"新建文件夹：{input.Name}", DocumentLogType.AddFolder);
        return folderId;
    }

    public async Task Rename(RenameDocumentInput input)
    {
        var entity = await _documentAccessService.GetDocumentForWrite(input.Id);
        var finalName = entity.NodeType == DocumentNodeType.File
            ? DocumentConst.ComposeRenamedFileName(input.Name, entity.Suffix)
            : input.Name?.Trim();
        await EnsureUniqueName(entity.ParentId, finalName, entity.Id);
        entity.Name = finalName;
        entity.Label = input.Label;
        entity.Remark = input.Remark;
        PrepareUpdate(entity);
        await Context.Updateable(entity).ExecuteCommandAsync();
        await _documentLogService.TryWrite(entity.Id, "重命名", $"重命名为：{entity.Name}", DocumentLogType.Rename);
    }

    public async Task Move(MoveDocumentInput input)
    {
        if (input.Ids == null || input.Ids.Count == 0)
            return;

        BizDocument targetParent = null;
        if (input.TargetParentId > 0)
        {
            targetParent = await _documentAccessService.GetDocumentForRead(input.TargetParentId);
            if (targetParent.NodeType != DocumentNodeType.Folder)
                throw Oops.Bah("目标节点必须是文件夹");
        }
        else if (!UserManager.SuperAdmin)
        {
            throw Oops.Bah("无权移动到根目录");
        }

        var moveDocs = new List<BizDocument>();
        foreach (var id in input.Ids.Distinct())
        {
            var doc = await _documentAccessService.GetDocumentForWrite(id);
            if (_documentAccessService.IsProtectedRoot(doc))
                throw Oops.Bah("授权根目录不支持移动");
            if (targetParent != null && (targetParent.Id == doc.Id || targetParent.Ancestors.Contains($",{doc.Id},")))
                throw Oops.Bah("不能移动到自身或下级目录");
            await EnsureUniqueName(input.TargetParentId, doc.Name, doc.Id);
            moveDocs.Add(doc);
        }

        var result = await Tenant.UseTranAsync(async () =>
        {
            foreach (var doc in moveDocs)
            {
                var subtree = await _documentAccessService.GetSubtree(doc.Id);
                var newAncestors = BuildAncestors(targetParent);
                var newRootId = input.TargetParentId == 0 ? doc.RootId : targetParent.RootId;
                var oldPrefix = GetSelfPath(doc);
                var newPrefix = $"{newAncestors}{doc.Id},";

                // `Ancestors` 存的是整条祖先链，移动目录时要把整棵子树的祖先路径和 RootId 一次性重写，
                // 否则树查询、权限判断以及“是否移动到自身子节点”这类逻辑都会出现错乱。
                foreach (var item in subtree)
                {
                    if (item.Id == doc.Id)
                    {
                        item.ParentId = input.TargetParentId;
                        item.Ancestors = newAncestors;
                    }
                    else
                    {
                        item.Ancestors = item.Ancestors.Replace(oldPrefix, newPrefix);
                    }
                    item.RootId = newRootId;
                }
                PrepareUpdate(subtree);
                await Context.Updateable(subtree).ExecuteCommandAsync();
                await _documentLogService.Write(doc.Id, "移动文件", $"移动到目录：{(targetParent?.Name ?? "根目录")}", DocumentLogType.Move);
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("移动失败");
        }
    }

    public async Task Delete(BaseIdListInput input)
    {
        if (input.Ids == null || input.Ids.Count == 0)
            return;

        var result = await Tenant.UseTranAsync(async () =>
        {
            foreach (var id in input.Ids.Distinct())
            {
                var doc = await _documentAccessService.GetDocumentForWrite(id);
                var subtree = await _documentAccessService.GetSubtree(id);
                subtree.ForEach(it => it.IsDeleted = true);
                PrepareUpdate(subtree);
                await Context.Updateable(subtree).ExecuteCommandAsync();
                await _documentLogService.Write(doc.Id, "删除文件", $"删除：{doc.Name}", DocumentLogType.Delete);
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("删除失败");
        }
    }

    public async Task UploadFiles(UploadDocumentInput input)
    {
        if (input.ParentId == 0 && !UserManager.SuperAdmin)
            throw Oops.Bah("只有超级管理员可以上传到根目录");
        var parent = input.ParentId == 0 ? null : await _documentAccessService.GetDocumentForRead(input.ParentId);
        if (parent != null && parent.NodeType != DocumentNodeType.Folder)
            throw Oops.Bah("只能上传到文件夹");

        foreach (var file in input.Files)
        {
            var fileName = Path.GetFileName(file.FileName);
            await EnsureUniqueName(input.ParentId, fileName);
            var sysFile = await _documentStorageService.Upload(string.IsNullOrWhiteSpace(input.Engine) ? SysDictConst.FILE_ENGINE_LOCAL : input.Engine, file);
            var document = CreateFileEntity(input.ParentId, fileName, parent, sysFile, input.Engine);
            PrepareCreate(document);
            await Context.Insertable(document).ExecuteCommandAsync();
            await _documentLogService.TryWrite(document.Id, "上传文件", $"上传文件：{fileName}", DocumentLogType.UploadFile);
        }
    }

    public async Task UploadFolder(UploadDocumentInput input)
    {
        if (input.ParentId == 0 && !UserManager.SuperAdmin)
            throw Oops.Bah("只有超级管理员可以上传到根目录");
        var parent = input.ParentId == 0 ? null : await _documentAccessService.GetDocumentForRead(input.ParentId);
        if (parent != null && parent.NodeType != DocumentNodeType.Folder)
            throw Oops.Bah("只能上传到文件夹");

        var folderCache = new Dictionary<string, BizDocument>();
        if (parent != null)
            folderCache[$"{parent.Id}:."] = parent;

        for (var i = 0; i < input.Files.Count; i++)
        {
            var file = input.Files[i];
            var relativePath = input.RelativePaths.Count > i ? input.RelativePaths[i] : file.FileName;
            relativePath = NormalizeRelativePath(relativePath);
            var segments = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                continue;

            var currentParent = parent;
            var currentParentId = input.ParentId;
            var currentPath = ".";
            for (var segmentIndex = 0; segmentIndex < segments.Length - 1; segmentIndex++)
            {
                var segment = segments[segmentIndex];
                currentPath = currentPath == "." ? segment : $"{currentPath}/{segment}";
                var key = $"{currentParentId}:{currentPath}";

                // 上传文件夹时浏览器只会提交文件流和相对路径，这里通过缓存避免同一路径重复建目录。
                if (!folderCache.TryGetValue(key, out var folder))
                {
                    folder = await GetOrCreateFolder(currentParentId, segment, currentParent);
                    folderCache[key] = folder;
                }
                currentParent = folder;
                currentParentId = folder.Id;
            }

            var fileName = segments[^1];
            await EnsureUniqueName(currentParentId, fileName);
            var sysFile = await _documentStorageService.Upload(string.IsNullOrWhiteSpace(input.Engine) ? SysDictConst.FILE_ENGINE_LOCAL : input.Engine, file);
            var document = CreateFileEntity(currentParentId, fileName, currentParent, sysFile, input.Engine);
            PrepareCreate(document);
            await Context.Insertable(document).ExecuteCommandAsync();
            await _documentLogService.TryWrite(document.Id, "上传文件夹", $"上传文件：{relativePath}", DocumentLogType.UploadFolder);
        }
    }

    public async Task<FileStreamResult> Download(BaseIdInput input)
    {
        var entity = await _documentAccessService.GetDocumentForRead(input.Id);
        if (entity.NodeType != DocumentNodeType.File || entity.FileId == null)
            throw Oops.Bah("当前节点不是文件");
        return await _documentStorageService.Download(entity.FileId.Value);
    }

    public async Task<DocumentPreviewOutput> Preview(BaseIdInput input)
    {
        var entity = await _documentAccessService.GetDocumentForRead(input.Id);
        if (entity.NodeType != DocumentNodeType.File || entity.FileId == null)
            throw Oops.Bah("当前节点不是文件");
        var file = await Context.Queryable<SysFile>().Where(it => it.Id == entity.FileId.Value).FirstAsync();
        if (file == null)
            throw Oops.Bah("文件不存在");
        return await _documentStorageService.Preview(file);
    }

    public async Task<DocumentGrantOutput> GrantDetail(BaseIdInput input)
    {
        var entity = await _documentAccessService.GetDocumentForRead(input.Id);
        if (!_documentAccessService.IsProtectedRoot(entity))
            throw Oops.Bah("只有授权根目录可分配权限");

        var userRelations = await _relationService.GetRelationListByTargetIdAndCategory(entity.Id.ToString(), CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER);
        var userIds = userRelations.Select(it => it.ObjectId).Distinct().ToList();
        var users = userIds.Count > 0
            ? await _sysUserService.GetUserListByIdList(new IdListInput { IdList = userIds })
            : new List<UserSelectorOutPut>();

        var roleRelations = await _relationService.GetRelationListByTargetIdAndCategory(entity.Id.ToString(), CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE);
        var roleIds = roleRelations.Select(it => it.ObjectId).Distinct().ToList();
        var roles = roleIds.Count > 0
            ? (await _sysRoleService.GetRoleListByIdList(new IdListInput { IdList = roleIds })).Adapt<List<RoleSelectorOutPut>>()
            : new List<RoleSelectorOutPut>();

        return new DocumentGrantOutput
        {
            Id = entity.Id,
            Users = users,
            Roles = roles
        };
    }

    public async Task GrantUsers(GrantDocumentUsersInput input)
    {
        await EnsureCanGrant(input.Id);
        foreach (var userId in input.UserIds.Distinct())
        {
            var user = await _sysUserService.GetUserById(userId);
            if (user == null)
                throw Oops.Bah($"用户不存在:{userId}");
        }
        await SaveGrant(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER, input.Id, input.UserIds.Select(it => it.ToString()).ToList());
        await _documentLogService.TryWrite(input.Id, "分配用户", $"分配用户数量：{input.UserIds.Distinct().Count()}", DocumentLogType.Grant);
    }

    public async Task GrantRoles(GrantDocumentRolesInput input)
    {
        await EnsureCanGrant(input.Id);
        var roles = await _sysRoleService.GetRoleListByIdList(new IdListInput { IdList = input.RoleIds.Distinct().ToList() });
        if (roles.Count != input.RoleIds.Distinct().Count())
            throw Oops.Bah("存在无效角色");
        await SaveGrant(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE, input.Id, input.RoleIds.Select(it => it.ToString()).ToList());
        await _documentLogService.TryWrite(input.Id, "分配角色", $"分配角色数量：{input.RoleIds.Distinct().Count()}", DocumentLogType.Grant);
    }

    private async Task SaveGrant(string category, long documentId, List<string> objectIds)
    {
        var result = await Tenant.UseTranAsync(async () =>
        {
            await Context.Deleteable<SysRelation>().Where(it => it.Category == category && it.TargetId == documentId.ToString()).ExecuteCommandAsync();
            if (objectIds.Count > 0)
            {
                var relations = objectIds.Select(it => new SysRelation
                {
                    ObjectId = long.Parse(it),
                    TargetId = documentId.ToString(),
                    Category = category
                }).ToList();
                await Context.Insertable(relations).ExecuteCommandAsync();
            }
        });
        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("分配失败");
        }
        await _relationService.RefreshCache(category);
    }

    private async Task EnsureCanGrant(long documentId)
    {
        if (!UserManager.SuperAdmin)
            throw Oops.Bah("只有超级管理员可以分配目录");
        var entity = await _documentAccessService.GetDocumentForRead(documentId);
        if (!_documentAccessService.IsProtectedRoot(entity))
            throw Oops.Bah("只有授权根目录可分配权限");
    }

    private async Task<BizDocument> GetOrCreateFolder(long parentId, string folderName, BizDocument parent)
    {
        var entity = await Context.Queryable<BizDocument>()
            .Where(it => it.ParentId == parentId && it.Name == folderName && it.Visible && !it.IsDeleted && it.NodeType == DocumentNodeType.Folder)
            .FirstAsync();
        if (entity != null)
            return entity;

        var folder = CreateFolderEntity(parentId, folderName, string.Empty, string.Empty, parent);
        PrepareCreate(folder);
        await Context.Insertable(folder).ExecuteCommandAsync();
        var insertEntity = folder;
        if (parentId == 0)
        {
            insertEntity.RootId = insertEntity.Id;
            await Context.Updateable(insertEntity).UpdateColumns(it => new { it.RootId }).ExecuteCommandAsync();
        }
        return insertEntity;
    }

    private BizDocument CreateFolderEntity(long parentId, string name, string label, string remark, BizDocument parent)
    {
        return new BizDocument
        {
            ParentId = parentId,
            RootId = parentId == 0 ? 0 : parent.RootId,
            Ancestors = BuildAncestors(parent),
            Name = name.Trim(),
            NodeType = DocumentNodeType.Folder,
            Label = label,
            Remark = remark,
            IsDeleted = false,
            Visible = true
        };
    }

    private BizDocument CreateFileEntity(long parentId, string fileName, BizDocument parent, SysFile sysFile, string engine)
    {
        return new BizDocument
        {
            ParentId = parentId,
            RootId = parentId == 0 ? 0 : parent.RootId,
            Ancestors = BuildAncestors(parent),
            Name = fileName,
            NodeType = DocumentNodeType.File,
            FileId = sysFile.Id,
            Engine = string.IsNullOrWhiteSpace(engine) ? sysFile.Engine : engine,
            SizeKb = sysFile.SizeKb,
            Suffix = DocumentConst.NormalizeSuffix(sysFile.Suffix),
            Remark = string.Empty,
            IsDeleted = false,
            Visible = true
        };
    }

    private async Task EnsureUniqueName(long parentId, string name, long? excludeId = null)
    {
        var exist = await Context.Queryable<BizDocument>()
            .Where(it => it.ParentId == parentId && it.Name == name.Trim() && it.Visible && !it.IsDeleted)
            .WhereIF(excludeId != null, it => it.Id != excludeId.Value)
            .AnyAsync();
        if (exist)
            throw Oops.Bah($"同级目录已存在同名文件：{name}");
    }

    private string BuildAncestors(BizDocument parent)
    {
        return parent == null ? ",0," : $"{parent.Ancestors}{parent.Id},";
    }

    private string GetSelfPath(BizDocument document)
    {
        return $"{document.Ancestors}{document.Id},";
    }

    private DocumentTreeOutput MapTree(BizDocument document)
    {
        return new DocumentTreeOutput
        {
            Id = document.Id,
            ParentId = document.ParentId,
            RootId = document.RootId,
            Name = document.Name,
            IsRoot = document.Id == document.RootId,
            Children = document.Children.Select(MapTree).ToList()
        };
    }

    private async Task<List<DocumentOutput>> FillDocumentOutputs(List<DocumentOutput> outputs)
    {
        var fileIds = outputs.Where(it => it.FileId != null).Select(it => it.FileId!.Value).Distinct().ToList();
        var fileMap = fileIds.Count > 0
            ? (await Context.Queryable<SysFile>().Where(it => fileIds.Contains(it.Id)).ToListAsync()).ToDictionary(it => it.Id, it => it)
            : new Dictionary<long, SysFile>();
        var isSuperAdmin = await _documentAccessService.IsSuperAdmin();

        foreach (var item in outputs)
        {
            // 前端按钮显隐直接依赖这些布尔字段，这里统一做一层“后端口径”的能力收敛。
            item.IsRoot = item.Id == item.RootId;
            item.CanGrant = isSuperAdmin && item.IsRoot;
            item.CanUpload = item.NodeType == DocumentNodeType.Folder;
            item.CanRename = isSuperAdmin || !item.IsRoot;
            item.CanMove = isSuperAdmin || !item.IsRoot;
            item.CanDelete = isSuperAdmin || !item.IsRoot;
            item.FileTypeLabel = DocumentConst.GetFileTypeLabel(item.NodeType, item.Suffix);
            item.CreateUserName ??= item.CreateUser;
            item.UpdateUserName ??= item.UpdateUser;
            if (item.SizeKb != null)
                item.SizeInfo = GetSizeInfo(item.SizeKb.Value);
            if (item.FileId != null && fileMap.TryGetValue(item.FileId.Value, out var file))
                item.Thumbnail = file.Thumbnail;
        }

        return outputs;
    }

    private string GetSizeInfo(long sizeKb)
    {
        var bytes = sizeKb * 1024d;
        if (bytes >= 1024 * 1024)
            return $"{Math.Round(bytes / 1024 / 1024, 2)} MB";
        if (bytes >= 1024)
            return $"{Math.Round(bytes / 1024, 2)} KB";
        return $"{bytes:0} B";
    }

    private string NormalizeRelativePath(string value)
    {
        return value.Replace('\\', '/').Trim('/');
    }

    private static DateTime? NormalizeRangeEnd(DateTime? value)
    {
        if (value == null)
            return null;

        return value.Value.TimeOfDay == TimeSpan.Zero ? value.Value.Date.AddDays(1).AddTicks(-1) : value;
    }

    private ISugarQueryable<BizDocument> ApplyFileTypeFilter(ISugarQueryable<BizDocument> query, DocumentPageInput input)
    {
        var fileType = input.FileType?.Trim();
        if (string.IsNullOrWhiteSpace(fileType))
        {
            return query.WhereIF(input.NodeType != null, it => it.NodeType == input.NodeType);
        }

        if (fileType == DocumentConst.FILE_TYPE_FOLDER)
            return query.Where(it => it.NodeType == DocumentNodeType.Folder);

        if (fileType == DocumentConst.FILE_TYPE_FILE)
        {
            return query.Where(it =>
                it.NodeType == DocumentNodeType.File &&
                (string.IsNullOrEmpty(it.Suffix) || !DocumentConst.KNOWN_FILE_SUFFIXES.Contains(SqlFunc.ToLower(it.Suffix))));
        }

        var suffixes = DocumentConst.GetFileTypeSuffixes(fileType);
        if (suffixes.Length == 0)
            return query.WhereIF(input.NodeType != null, it => it.NodeType == input.NodeType);

        return query.Where(it =>
            it.NodeType == DocumentNodeType.File &&
            !string.IsNullOrEmpty(it.Suffix) &&
            suffixes.Contains(SqlFunc.ToLower(it.Suffix)));
    }

    private void PrepareCreate(BizDocument entity)
    {
        entity.Id = CommonUtils.GetSingleId();
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

    private void PrepareUpdate(BizDocument entity)
    {
        entity.UpdateTime = DateTime.Now;
        entity.UpdateUserId = UserManager.UserId;
        entity.UpdateUser = UserManager.Name;
    }

    private void PrepareUpdate(IEnumerable<BizDocument> entities)
    {
        foreach (var entity in entities)
            PrepareUpdate(entity);
    }
}
