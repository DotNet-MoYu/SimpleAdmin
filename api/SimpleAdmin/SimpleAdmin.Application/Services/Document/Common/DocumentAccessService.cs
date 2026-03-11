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

#nullable enable

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IDocumentAccessService"/>
public class DocumentAccessService : DbRepository<BizDocument>, IDocumentAccessService
{
    private readonly IRelationService _relationService;

    public DocumentAccessService(IRelationService relationService)
    {
        _relationService = relationService;
    }

    public Task<bool> IsSuperAdmin()
    {
        return Task.FromResult(UserManager.SuperAdmin);
    }

    public bool IsProtectedRoot(BizDocument document)
    {
        return document is { Id: > 0 } && document.Id == document.RootId;
    }

    public async Task<List<long>?> GetAuthorizedRootIds()
    {
        if (UserManager.SuperAdmin)
            return null;

        var rootIds = new HashSet<long>();

        // 用户可以通过“直接授权根目录”或“角色继承根目录授权”两条链路获得访问权限，这里统一合并成 RootId 集合。
        var userRelations = await _relationService.GetRelationListByObjectIdAndCategory(UserManager.UserId, CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_USER);
        userRelations.ForEach(it =>
        {
            if (long.TryParse(it.TargetId, out var folderId))
                rootIds.Add(folderId);
        });

        var roleRelations = await _relationService.GetRelationListByObjectIdAndCategory(UserManager.UserId, CateGoryConst.RELATION_SYS_USER_HAS_ROLE);
        var roleIds = roleRelations.Select(it => it.TargetId.ToLong()).Where(it => it > 0).Distinct().ToList();
        if (roleIds.Count > 0)
        {
            var folderRelations = await _relationService.GetRelationListByObjectIdListAndCategory(roleIds, CateGoryConst.RELATION_BIZ_DOCUMENT_FOLDER_TO_ROLE);
            folderRelations.ForEach(it =>
            {
                if (long.TryParse(it.TargetId, out var folderId))
                    rootIds.Add(folderId);
            });
        }

        return rootIds.ToList();
    }

    public async Task<BizDocument> GetDocumentForRead(long id, bool includeDeleted = false, bool includeInvisible = false)
    {
        var entity = await Context.Queryable<BizDocument>()
            .Where(it => it.Id == id)
            .WhereIF(!includeInvisible, it => it.Visible)
            .WhereIF(!includeDeleted, it => !it.IsDeleted)
            .FirstAsync();

        if (entity == null)
            throw Oops.Bah("文件不存在");

        var rootIds = await GetAuthorizedRootIds();
        if (rootIds != null && !rootIds.Contains(entity.RootId))
            throw Oops.Bah("您无权访问该文件");

        return entity;
    }

    public async Task<BizDocument> GetDocumentForWrite(long id, bool includeDeleted = false, bool includeInvisible = false, bool allowProtectedRoot = false)
    {
        var entity = await GetDocumentForRead(id, includeDeleted, includeInvisible);
        if (!UserManager.SuperAdmin && !allowProtectedRoot && IsProtectedRoot(entity))
            throw Oops.Bah("无权操作授权根目录");
        return entity;
    }

    public async Task<List<BizDocument>> GetSubtree(long id, bool includeInvisible = false)
    {
        var self = await Context.Queryable<BizDocument>()
            .Where(it => it.Id == id)
            .WhereIF(!includeInvisible, it => it.Visible)
            .FirstAsync();
        if (self == null)
            return new List<BizDocument>();

        var keyword = $"%,{id},%";

        // `Ancestors` 采用 `,0,1,2,` 这种冗余路径，利用 LIKE 可以一次查出整棵子树。
        return await Context.Queryable<BizDocument>()
            .Where(it => it.Id == id || SqlFunc.Like(it.Ancestors, keyword))
            .WhereIF(!includeInvisible, it => it.Visible)
            .OrderBy(it => it.Ancestors)
            .OrderBy(it => it.Id)
            .ToListAsync();
    }
}

#nullable restore
