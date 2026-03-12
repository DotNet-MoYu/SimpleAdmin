// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using SimpleAdmin.Core.Utils;
using Microsoft.Extensions.Logging;

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IDocumentLogService"/>
public class DocumentLogService : DbRepository<BizDocumentLog>, IDocumentLogService
{
    private readonly IDocumentAccessService _documentAccessService;
    private readonly ILogger<DocumentLogService> _logger;

    public DocumentLogService(IDocumentAccessService documentAccessService, ILogger<DocumentLogService> logger)
    {
        _documentAccessService = documentAccessService;
        _logger = logger;
    }

    public async Task<SqlSugarPagedList<DocumentLogOutput>> Page(DocumentLogPageInput input)
    {
        var endTime = NormalizeRangeEnd(input.EndTime);
        var query = Context.Queryable<BizDocumentLog>()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), it => it.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserName), it => it.UserName.Contains(input.UserName))
            .WhereIF(input.Type != null, it => it.Type == input.Type)
            .WhereIF(input.StartTime != null && endTime != null, it => SqlFunc.Between(it.DoTime, input.StartTime, endTime));

        var rootIds = await _documentAccessService.GetAuthorizedRootIds();
        if (rootIds != null)
        {
            if (rootIds.Count == 0)
            {
                query = query.Where(it => it.Id == -1);
            }
            else
            {
                // 日志本身只存 DocumentId，这里需要把“用户有权访问的根目录”映射成具体文档 ID 列表。
                var docIds = await Context.Queryable<BizDocument>().Where(it => rootIds.Contains(it.RootId) && it.Visible).Select(it => it.Id).ToListAsync();
                query = query.Where(it => it.DocumentId != null && docIds.Contains(it.DocumentId.Value));
            }
        }

        return await query.OrderBy(it => it.DoTime, OrderByType.Desc).Select<DocumentLogOutput>().ToPagedListAsync(input.PageNum, input.PageSize);
    }

    public async Task Empty()
    {
        if (!UserManager.SuperAdmin)
            throw Oops.Bah("只有超级管理员可以清空日志");
        await DeleteAsync(it => true);
    }

    public async Task Write(long? documentId, string name, string detail, DocumentLogType type)
    {
        var log = new BizDocumentLog
        {
            Id = CommonUtils.GetSingleId(),
            DocumentId = documentId,
            Name = name,
            Detail = detail,
            Type = type,
            UserId = UserManager.UserId,
            UserName = UserManager.Name,
            DoTime = DateTime.Now
        };
        await InsertAsync(log);
    }

    public async Task TryWrite(long? documentId, string name, string detail, DocumentLogType type)
    {
        try
        {
            await Write(documentId, name, detail, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写入文件日志失败，DocumentId={DocumentId}, Name={Name}, Type={Type}", documentId, name, type);
        }
    }

    private static DateTime? NormalizeRangeEnd(DateTime? value)
    {
        if (value == null)
            return null;

        return value.Value.TimeOfDay == TimeSpan.Zero ? value.Value.Date.AddDays(1).AddTicks(-1) : value;
    }
}
