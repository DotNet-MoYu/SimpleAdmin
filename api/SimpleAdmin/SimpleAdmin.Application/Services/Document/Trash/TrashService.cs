// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Microsoft.Extensions.Logging;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="ITrashService"/>
public class TrashService : DbRepository<BizDocument>, ITrashService
{
    private readonly ILogger<TrashService> _logger;
    private readonly IDocumentAccessService _documentAccessService;
    private readonly IDocumentLogService _documentLogService;
    private readonly IRelationService _relationService;

    public TrashService(ILogger<TrashService> logger, IDocumentAccessService documentAccessService, IDocumentLogService documentLogService, IRelationService relationService)
    {
        _logger = logger;
        _documentAccessService = documentAccessService;
        _documentLogService = documentLogService;
        _relationService = relationService;
    }

    public async Task<SqlSugarPagedList<DocumentOutput>> Page(DocumentPageInput input)
    {
        var createTimeEnd = NormalizeRangeEnd(input.CreateTimeEnd);
        var updateTimeEnd = NormalizeRangeEnd(input.UpdateTimeEnd);
        var rootIds = await _documentAccessService.GetAuthorizedRootIds();
        var query = Context.Queryable<BizDocument>()
            .Where(it => it.Visible && it.IsDeleted)
            .WhereIF(rootIds != null && rootIds.Count > 0, it => rootIds.Contains(it.RootId))
            .WhereIF(rootIds is { Count: 0 }, it => it.Id == -1)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), it => it.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Label), it => it.Label == input.Label)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Suffix), it => it.Suffix == input.Suffix)
            .WhereIF(input.CreateTimeStart != null && createTimeEnd != null, it => SqlFunc.Between(it.CreateTime, input.CreateTimeStart, createTimeEnd))
            .WhereIF(input.UpdateTimeStart != null && updateTimeEnd != null, it => SqlFunc.Between(it.UpdateTime, input.UpdateTimeStart, updateTimeEnd))
            .OrderBy(it => it.UpdateTime, OrderByType.Desc);
        query = ApplyFileTypeFilter(query, input);
        var outputQuery = query.Select<DocumentOutput>();
        var page = await outputQuery.ToPagedListAsync(input.PageNum, input.PageSize);
        foreach (var item in page.List)
        {
            item.IsRoot = item.Id == item.RootId;
            item.CanDelete = true;
            item.CanRename = false;
            item.CanMove = false;
            item.CanGrant = false;
            item.CanUpload = false;
            item.FileTypeLabel = DocumentConst.GetFileTypeLabel(item.NodeType, item.Suffix);
            item.CreateUserName ??= item.CreateUser;
            item.UpdateUserName ??= item.UpdateUser;
            if (item.SizeKb != null)
                item.SizeInfo = GetSizeInfo(item.SizeKb.Value);
        }
        return page;
    }

    public async Task Recover(BaseIdListInput input)
    {
        if (input.Ids == null || input.Ids.Count == 0)
            return;

        var result = await Tenant.UseTranAsync(async () =>
        {
            foreach (var id in input.Ids.Distinct())
            {
                var entity = await _documentAccessService.GetDocumentForRead(id, includeDeleted: true);
                var subtree = await _documentAccessService.GetSubtree(id);
                var self = subtree.FirstOrDefault(it => it.Id == entity.Id);
                if (self == null)
                    throw Oops.Bah("恢复节点不存在");

                var newName = await GetRecoverName(entity);
                if (newName != entity.Name)
                    self.Name = newName;

                // 恢复时要同时恢复整个子树，避免父目录恢复后子文件仍停留在回收站。
                subtree.ForEach(it => it.IsDeleted = false);
                PrepareUpdate(subtree);
                await Context.Updateable(subtree).ExecuteCommandAsync();
                await _documentLogService.Write(entity.Id, "恢复文件", $"恢复：{self.Name}", DocumentLogType.Restore);
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("恢复失败");
        }
    }

    public async Task DeletePermanent(BaseIdListInput input)
    {
        if (input.Ids == null || input.Ids.Count == 0)
            return;

        var result = await Tenant.UseTranAsync(async () =>
        {
            var rootIds = new HashSet<long>();
            foreach (var id in input.Ids.Distinct())
            {
                var entity = await _documentAccessService.GetDocumentForRead(id, includeDeleted: true);
                var subtree = await _documentAccessService.GetSubtree(id);
                subtree.ForEach(it => it.Visible = false);
                await Context.Updateable(subtree).UpdateColumns(it => new { it.Visible }).ExecuteCommandAsync();
                if (entity.Id == entity.RootId)
                    rootIds.Add(entity.Id);
                await _documentLogService.Write(entity.Id, "永久删除", $"永久删除：{entity.Name}", DocumentLogType.PermanentDelete);
            }
            if (rootIds.Count > 0)
            {
                var targetIds = rootIds.Select(it => it.ToString()).ToList();
                await Context.Deleteable<SysRelation>()
                    .Where(it => targetIds.Contains(it.TargetId) &&
                                 (it.Category == CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER ||
                                  it.Category == CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE))
                    .ExecuteCommandAsync();
                await _relationService.RefreshCache(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER);
                await _relationService.RefreshCache(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE);
            }
        });

        if (!result.IsSuccess)
        {
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Bah("永久删除失败");
        }
    }

    public async Task Empty()
    {
        var rootIds = await _documentAccessService.GetAuthorizedRootIds();
        var query = Context.Queryable<BizDocument>().Where(it => it.Visible && it.IsDeleted);
        query = rootIds == null
            ? query
            : rootIds.Count == 0 ? query.Where(it => it.Id == -1) : query.Where(it => rootIds.Contains(it.RootId));
        var docs = await query.ToListAsync();
        docs.ForEach(it => it.Visible = false);
        await Context.Updateable(docs).UpdateColumns(it => new { it.Visible }).ExecuteCommandAsync();

        var deletedRootIds = docs.Where(it => it.Id == it.RootId).Select(it => it.Id.ToString()).Distinct().ToList();
        if (deletedRootIds.Count > 0)
        {
            await Context.Deleteable<SysRelation>()
                .Where(it => deletedRootIds.Contains(it.TargetId) &&
                             (it.Category == CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER ||
                              it.Category == CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE))
                .ExecuteCommandAsync();
            await _relationService.RefreshCache(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER);
            await _relationService.RefreshCache(CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE);
        }

        await _documentLogService.TryWrite(null, "清空回收站", "清空回收站", DocumentLogType.EmptyTrash);
    }

    private async Task<string> GetRecoverName(BizDocument entity)
    {
        var name = entity.Name;
        var index = 1;
        while (await Context.Queryable<BizDocument>().Where(it => it.ParentId == entity.ParentId && it.Name == name && it.Visible && !it.IsDeleted && it.Id != entity.Id).AnyAsync())
        {
            name = entity.NodeType == DocumentNodeType.Folder
                ? $"{entity.Name}({index})"
                : $"{Path.GetFileNameWithoutExtension(entity.Name)}({index}){Path.GetExtension(entity.Name)}";
            index++;
        }
        return name;
    }

    private void PrepareUpdate(IEnumerable<BizDocument> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdateTime = DateTime.Now;
            entity.UpdateUserId = UserManager.UserId;
            entity.UpdateUser = UserManager.Name;
        }
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

    private string GetSizeInfo(long sizeKb)
    {
        var bytes = sizeKb * 1024d;
        if (bytes >= 1024 * 1024)
            return $"{Math.Round(bytes / 1024 / 1024, 2)} MB";
        if (bytes >= 1024)
            return $"{Math.Round(bytes / 1024, 2)} KB";
        return $"{bytes:0} B";
    }

    private static DateTime? NormalizeRangeEnd(DateTime? value)
    {
        if (value == null)
            return null;

        // 前端日期组件通常只传 yyyy-MM-dd，绑定到 DateTime 后会落在当天 00:00:00。
        // 这里统一扩到当天末尾，避免“结束日期当天的数据被漏掉”。
        return value.Value.TimeOfDay == TimeSpan.Zero ? value.Value.Date.AddDays(1).AddTicks(-1) : value;
    }
}
