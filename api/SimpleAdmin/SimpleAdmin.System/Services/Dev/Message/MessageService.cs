namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IMessageService"/>
/// </summary>
public class MessageService : DbRepository<DevMessage>, IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IRelationService _relationService;
    private readonly IEventPublisher _eventPublisher;

    public MessageService(ILogger<MessageService> logger, ISimpleCacheService simpleCacheService, IRelationService relationService, IEventPublisher eventPublisher)
    {
        this._logger = logger;
        this._simpleCacheService = simpleCacheService;
        this._relationService = relationService;
        this._eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevMessage>> Page(MessagePageInput input)
    {
        var query = Context.Queryable<DevMessage>()
                           .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                           .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Subject.Contains(input.SearchKey) || it.Content.Contains(input.SearchKey))//根据关键字查询
                           .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                           .OrderBy(it => it.CreateTime, OrderByType.Desc)
                           ;

        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevMessage>> MyMessagePage(MessagePageInput input, long userId)
    {
        var query = Context.Queryable<DevMessageUser>().LeftJoin<DevMessage>((u, m) => u.MessageId == m.Id)
              .Where((u, m) => u.IsDelete == false && u.UserId == userId)
              .WhereIF(!string.IsNullOrEmpty(input.Category), (u, m) => m.Category == input.Category)//根据分类查询
              .OrderBy((u, m) => u.Read, OrderByType.Asc)
              .OrderBy((u, m) => m.CreateTime, OrderByType.Desc)
              .Select((u, m) => new DevMessage
              {
                  Id = m.Id.SelectAll(),
                  Read = u.Read,
              })
             ;
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }
    /// <inheritdoc />
    public async Task Send(MessageSendInput input)
    {
        var message = input.Adapt<DevMessage>();//实体转换
        var messageUsers = new List<DevMessageUser>();
        input.ReceiverIdList.ForEach(it =>
        {
            //遍历用户ID列表，生成拓展列表
            messageUsers.Add(new DevMessageUser { UserId = it, Read = false, IsDelete = false });
        });

        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            message = await InsertReturnEntityAsync(message);//添加消息
            messageUsers.ForEach(it => it.MessageId = message.Id);//添加关系
            await Context.Insertable(messageUsers).ExecuteCommandAsync();
        });
        if (result.IsSuccess)//如果成功了
        {
            await _eventPublisher.PublishAsync(EventSubscriberConst.NewMessage, new NewMessageEvent
            {
                UserIds = input.ReceiverIdList.Select(it => it.ToString()).ToList(),
                Message = input.Subject
            }); //通知用户有新的消息

        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }
    }

    /// <inheritdoc />
    public async Task<MessageDetailOutPut> Detail(BaseIdInput input, bool isSelf = false)
    {
        //获取消息
        var message = await GetFirstAsync(it => it.Id == input.Id);
        if (message != null)
        {
            var messageDetail = message.Adapt<MessageDetailOutPut>();//实体转换
            var messageUserRep = ChangeRepository<DbRepository<DevMessageUser>>();//切换仓储
            var messageUsers = await messageUserRep.GetListAsync(it => it.MessageId == message.Id);
            var myMessage = messageUsers.Where(it => it.UserId == UserManager.UserId && it.MessageId == input.Id).FirstOrDefault();//查询是否发给自己
            if (myMessage != null)
            {
                myMessage.Read = true;//设置已读
                await messageUserRep.UpdateAsync(myMessage);//修改状态
            }
            if (!isSelf)//如果不是自己则把所有的用户都列出来
            {
                var userIds = messageUsers.Select(it => it.UserId).ToList();//获取用户ID列表
                var userInfos = await Context.Queryable<SysUser>()
                    .Where(it => userIds.Contains(it.Id)).Select(it => new { it.Id, it.Name }).ToListAsync();//获取用户姓名信息列表

                //遍历关系
                messageUsers.ForEach(messageUser =>
                {
                    var user = userInfos.Where(u => u.Id == messageUser.UserId).FirstOrDefault();//获取用户信息
                    if (user != null)
                    {
                        //添加到已读列表
                        messageDetail.ReceiveInfoList.Add(new MessageDetailOutPut.ReceiveInfo
                        {
                            ReceiveUserId = user.Id,
                            ReceiveUserName = user.Name,
                            Read = messageUser.Read
                        });
                    }
                    else//用户ID没找到
                    {
                        //添加到已读列表
                        messageDetail.ReceiveInfoList.Add(new MessageDetailOutPut.ReceiveInfo
                        {
                            ReceiveUserId = messageUser.UserId,
                            ReceiveUserName = "未知用户",
                            Read = messageUser.Read
                        });
                    }
                });
            }
            return messageDetail;
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                await DeleteAsync(it => ids.Contains(it.Id));
                await Context.Deleteable<DevMessageUser>().Where(it => ids.Contains(it.MessageId)).ExecuteCommandAsync();
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
        var messageUserRep = ChangeRepository<DbRepository<DevMessageUser>>();//切换仓储
        await Context.Deleteable<DevMessageUser>().Where(it => it.UserId == userId && it.MessageId == input.Id).IsLogic().ExecuteCommandAsync();//逻辑删除
    }
    /// <inheritdoc />
    public async Task<int> UnReadCount(long userId)
    {
        var messageUserRep = ChangeRepository<DbRepository<DevMessageUser>>();//切换仓储
        //获取未读数量
        var unRead = await messageUserRep.CountAsync(it => it.UserId == userId && it.Read == false && it.IsDelete == false);
        return unRead;
    }

}
