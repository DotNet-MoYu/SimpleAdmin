// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Masuit.Tools.DateTimeExt;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IMessageService"/>
/// </summary>
public class MessageService : DbRepository<SysMessage>, IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IRelationService _relationService;
    private readonly IEventPublisher _eventPublisher;

    public MessageService(ILogger<MessageService> logger, ISimpleCacheService simpleCacheService, IRelationService relationService,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _relationService = relationService;
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysMessage>> Page(MessagePageInput input)
    {
        var query = Context.Queryable<SysMessage>().WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.Subject.Contains(input.SearchKey) || it.Content.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
            .OrderBy(it => it.CreateTime, OrderByType.Desc);

        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysMessage>> MyMessagePage(MessagePageInput input, long userId)
    {
        var query = Context.Queryable<SysMessageUser>().LeftJoin<SysMessage>((u, m) => u.MessageId == m.Id)
            .Where((u, m) => u.IsDelete == false && u.UserId == userId && u.Status == SysDictConst.MESSAGE_STATUS_ALREADY)
            .WhereIF(!string.IsNullOrEmpty(input.Category), (u, m) => m.Category == input.Category)//根据分类查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                (u, m) => m.Subject.Contains(input.SearchKey) || m.Content.Contains(input.SearchKey))//根据关键字查询
            .OrderBy((u, m) => u.Read).OrderBy((u, m) => u.CreateTime, OrderByType.Desc).Select((u, m) => new SysMessage
            {
                Id = m.Id.SelectAll(),
                Read = u.Read
            });
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(MessageSendInput input)
    {
        CheckInput(input);
        var sysMessage = input.Adapt<SysMessage>();//实体转换
        sysMessage.Id = CommonUtils.GetSingleId();
        //如果是立即发送的直接修改状态
        if (sysMessage.SendWay == SysDictConst.SEND_WAY_NOW)
            sysMessage.Status = SysDictConst.MESSAGE_STATUS_ALREADY;//已发送
        var messageUsers = new List<SysMessageUser>();
        var userIds = new List<long>();
        //根据接收人获取需要发送的用户id
        switch (sysMessage.ReceiverType)
        {
            case SysDictConst.RECEIVER_TYPE_ALL:
                userIds = await Context.Queryable<SysUser>().Select(it => it.Id).ToListAsync();
                break;
            case SysDictConst.RECEIVER_TYPE_ROLE:
                var roleIds = sysMessage.ReceiverInfo.Select(it => it.Id.ToString()).ToList();//获取角色ID列表
                var roleUsers =
                    await _relationService.GetRelationListByTargetIdListAndCategory(roleIds, CateGoryConst.RELATION_SYS_USER_HAS_ROLE);//获取角色用户列表
                userIds = roleUsers.Select(it => it.ObjectId).ToList();//获取用户ID列表
                break;
            case SysDictConst.RECEIVER_TYPE_APPOINT:
                userIds = sysMessage.ReceiverInfo.Select(it => it.Id).ToList();
                break;
        }
        //去掉自己
        userIds = userIds.Where(it => it != UserManager.UserId).ToList();

        //遍历用户ID
        userIds.ForEach(userId =>
        {
            messageUsers.Add(new SysMessageUser
            {
                MessageId = sysMessage.Id,
                UserId = userId,
                Read = false,
                IsDelete = false,
                Status = sysMessage.SendWay == SysDictConst.SEND_WAY_NOW ? SysDictConst.MESSAGE_STATUS_READY : SysDictConst.MESSAGE_STATUS_ALREADY
            });
        });
        //事务
        var result = await Tenant.UseTranAsync(async () =>
        {
            await InsertAsync(sysMessage);//添加消息
            await Context.Insertable(messageUsers).ExecuteCommandAsync();
        });
        if (result.IsSuccess)//如果成功了
        {
            await _eventPublisher.PublishAsync(EventSubscriberConst.NEW_MESSAGE, new NewMessageEvent
            {
                Id = sysMessage.Id,
                SendWay = sysMessage.SendWay
            });//通知用户有新的消息
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }
    }

    /// <inheritdoc />
    public async Task Edit(MessageSendUpdateInput input)
    {
        CheckInput(input);
        var sysMessage = input.Adapt<SysMessage>();//实体转换
        await UpdateAsync(sysMessage);//更新数据
    }




    /// <inheritdoc />
    public async Task<SysMessage> Detail(MessageDetailInput input)
    {
        //获取消息
        var message = await GetFirstAsync(it => it.Id == input.Id);
        if (message != null && message.SendWay == SysDictConst.SEND_WAY_DELAY)
        {
            //delayTime等于发送时间减掉创建时间
            message.DelayTime = (int)(message.SendTime.GetTotalSeconds() - message.CreateTime.Value.GetTotalSeconds());
        }
        if (message != null && input.ShowReceiveInfo)
        {
            var messageUserRep = ChangeRepository<DbRepository<SysMessageUser>>();//切换仓储
            var messageUsers = await messageUserRep.GetListAsync(it => it.MessageId == message.Id);

            var userIds = messageUsers.Select(it => it.UserId).ToList();//获取用户ID列表
            var userInfos = await Context.Queryable<SysUser>().Where(it => userIds.Contains(it.Id)).Select(it => new { it.Id, it.Name })
                .ToListAsync();//获取用户姓名信息列表
            //遍历关系
            messageUsers.ForEach(messageUser =>
            {
                var user = userInfos.Where(u => u.Id == messageUser.UserId).FirstOrDefault();//获取用户信息
                if (user != null)
                {
                    //添加到已读列表
                    message.ReceiverDetail.Add(new ReceiverDetail
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Read = messageUser.Read
                    });
                }
            });
        }
        //设置已读
        if (input.Read)
        {
            await Context.Updateable<SysMessageUser>()
                .SetColumns(it => it.Read == true)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.MessageId == input.Id && it.Read == false && it.UserId == UserManager.UserId).ExecuteCommandAsync();
        }
        return message;
    }




    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                await DeleteAsync(it => ids.Contains(it.Id));
                await Context.Deleteable<SysMessageUser>().Where(it => ids.Contains(it.MessageId)).ExecuteCommandAsync();
            });
            if (!result.IsSuccess)//如果失败了
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }
    }

    /// <inheritdoc />
    public async Task DeleteMyMessage(BaseIdInput input, long userId)
    {
        var messageUserRep = ChangeRepository<DbRepository<SysMessageUser>>();//切换仓储
        await Context.Deleteable<SysMessageUser>().Where(it => it.UserId == userId && it.MessageId == input.Id).IsLogic().ExecuteCommandAsync();//逻辑删除
    }

    /// <inheritdoc />
    public async Task<List<MessageUnReadOutPut>> UnReadCount(long userId)
    {
        // 连表查询未读
        var unReadList = await Context.Queryable<SysMessage>().LeftJoin<SysMessageUser>((m, u) => u.MessageId == m.Id)
            .Where((m, u) => u.UserId == userId && u.Read == false && u.IsDelete == false)
            .Select<SysMessage>().ToListAsync();
        //根据消息分类分组
        var groupList = unReadList.GroupBy(it => it.Category).Select(it => new MessageUnReadOutPut
        {
            Category = it.Key,
            UnReadCount = it.Count()
        }).ToList();
        return groupList;
    }

    /// <inheritdoc />
    public async Task<List<SysMessage>> NewUnRead(long userId)
    {
        //根据消息分类分组,获取每组未读前五条
        var result = await Context.Queryable<SysMessage>().LeftJoin<SysMessageUser>((m, u) => u.MessageId == m.Id)
            .Where((m, u) => u.UserId == userId && u.IsDelete == false && u.Status == SysDictConst.MESSAGE_STATUS_ALREADY)
            .Select((m, u) => new SysMessage
            {
                Index2 = SqlFunc.RowNumber($"{m.Id} desc",
                    m.Category),//order by id partition by name，参考sqlsugar分组查询https://www.donet5.com/Doc/1/2243
                Subject = m.Subject,
                CreateTime = m.CreateTime,
                SendTime = m.SendTime,
                Category = m.Category,
                Read = u.Read
            })
            .MergeTable()//将结果合并成一个表
            .Where(it => it.Index2 <= 5)//取第五条
            .Mapper(it =>
            {
                it.SendTimeFormat = TimeHelper.FormatTimeAgo(it.SendTime);
            })
            .ToListAsync();
        return result;
    }

    /// <inheritdoc />
    public async Task<int> SetRead(MessageReadInput input)
    {
        //如果ID列表为空，就是全部已读
        if (input.Ids.Count == 0 && input.Category != null)
        {
            var result = await Context.Updateable<SysMessageUser>()
                .SetColumns(it => it.Read == true)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.UserId == UserManager.UserId && it.Read == false)
                .Where(it => SqlFunc.Subqueryable<SysMessage>().Where(s => s.Id == it.MessageId && s.Category == input.Category).Any())
                .ExecuteCommandAsync();
            return result;
        }
        else
        {
            return await Context.Updateable<SysMessageUser>()
                .SetColumns(it => it.Read == true)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.UserId == UserManager.UserId && input.Ids.Contains(it.MessageId))
                .Where(it => it.Read == false)
                .ExecuteCommandAsync();
        }
    }

    /// <inheritdoc />
    public async Task<int> SetDelete(MessageReadInput input)
    {
        //如果ID列表为空，就是全部删除
        if (input.Ids.Count == 0 && input.Category != null)
        {
            var result = await Context.Updateable<SysMessageUser>()
                .SetColumns(it => it.IsDelete == true)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.UserId == UserManager.UserId && it.IsDelete == false)
                .Where(it => SqlFunc.Subqueryable<SysMessage>().Where(s => s.Id == it.MessageId && s.Category == input.Category).Any())
                .ExecuteCommandAsync();
            return result;
        }
        else
        {
            return await Context.Updateable<SysMessageUser>()
                .SetColumns(it => it.IsDelete == true)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.UserId == UserManager.UserId && input.Ids.Contains(it.MessageId))
                .Where(it => it.IsDelete == false)
                .ExecuteCommandAsync();
        }
    }

    #region 方法

    /// <summary>
    /// 检查输入
    /// </summary>
    /// <param name="input"></param>
    private void CheckInput(MessageSendInput input)
    {
        if (input.Status == SysDictConst.MESSAGE_STATUS_ALREADY)
            throw Oops.Oh("已发送的消息不能修改");
        switch (input.SendWay)
        {
            case SysDictConst.SEND_WAY_NOW:
                input.SendTime = DateTime.Now;
                break;
            case SysDictConst.SEND_WAY_DELAY:
                if (input.CreateTime != null)
                    input.SendTime = input.CreateTime.Value.AddSeconds(input.DelayTime);
                else
                    input.SendTime = DateTime.Now.AddSeconds(input.DelayTime);
                break;
            case SysDictConst.SEND_WAY_SCHEDULE:
                if (input.SendTime < DateTime.Now)
                    throw Oops.Oh("发送时间不能小于当前时间");
                input.SendTime = input.SendTime;
                break;
        }
    }

    #endregion
}
