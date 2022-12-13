namespace SimpleAdmin.System
{
    /// <summary>
    /// <inheritdoc cref="IMessageService"/>
    /// </summary>
    public class MessageService : DbRepository<DevMessage>, IMessageService
    {
        private readonly ILogger<MessageService> _logger;
        private readonly IRelationService _relationService;

        public MessageService(ILogger<MessageService> logger, IRelationService relationService)
        {
            this._logger = logger;
            this._relationService = relationService;
        }

        /// <inheritdoc />
        public async Task<SqlSugarPagedList<DevMessage>> Page(MessagePageInput input)
        {
            List<DevRelation> relations = new List<DevRelation>();//用户消息关系列表
            if (input.ReceiveUserId > 0)//如果用户ID大于0
            {
                relations = await Context.Queryable<DevRelation>()//获取关系表中的站内信ID
                   .Where(it => it.Category == CateGoryConst.Relation_MSG_TO_USER && it.TargetId == input.ReceiveUserId.ToString())//根据分类和用户用户ID查询
                  .ToListAsync();

                input.Ids = relations.Select(it => it.ObjectId).ToList();
            }
            var query = Context.Queryable<DevMessage>()
                               .WhereIF(input.Ids != null, it => input.Ids.Contains(it.Id))//根据用户ID查询查询
                               .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                               .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Subject.Contains(input.SearchKey) || it.Content.Contains(input.SearchKey))//根据关键字查询
                               .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                               .OrderBy(it => it.CreateTime, OrderByType.Desc)
                               .Mapper(it =>
                               {
                                   //如果关系表数量大于0
                                   if (relations.Count > 0)
                                   {
                                       var relation = relations.Where(r => r.ObjectId == it.Id).FirstOrDefault();//获取关系
                                       if (relation != null)
                                           it.Read = relation.ExtJson.ToJsonEntity<RelationMsgUser>().Read;//获取已读状态

                                   }
                               });

            var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
            return pageInfo;
        }

        /// <inheritdoc />
        public async Task Send(MessageSendInput input)
        {
            var message = input.Adapt<DevMessage>();//实体转换
            var relations = new List<DevRelation>();
            input.ReceiverIdList.ForEach(it =>
            {
                //遍历用户ID列表，生成拓展列表
                relations.Add(new DevRelation { Category = CateGoryConst.Relation_MSG_TO_USER, TargetId = it.ToString(), ExtJson = new RelationMsgUser { }.ToJson() });
            });

            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                message = await InsertReturnEntityAsync(message);//添加消息
                relations.ForEach(it => it.ObjectId = message.Id);//添加关系
                await Context.Insertable(relations).ExecuteCommandAsync();
            });
            if (!result.IsSuccess)//如果失败了
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
                var relationRep = ChangeRepository<DbRepository<DevRelation>>();//切换仓促
                var relations = await relationRep.GetListAsync(it => it.ObjectId == message.Id && it.Category == CateGoryConst.Relation_MSG_TO_USER);
                var myMessage = relations.Where(it => it.TargetId == UserManager.UserId.ToString()).FirstOrDefault();//查询是否发给自己
                if (myMessage != null)
                {
                    myMessage.ExtJson = new RelationMsgUser { Read = true }.ToJson();//设置已读
                    await relationRep.UpdateAsync(myMessage);//修改状态
                }
                if (!isSelf)//如果不是自己则把所有的用户都列出来
                {
                    var userIds = relations.Select(it => it.TargetId.ToLong()).ToList();//获取用户ID列表
                    var userInfos = await Context.Queryable<SysUser>()
                        .Where(it => userIds.Contains(it.Id)).Select(it => new { it.Id, it.Name }).ToListAsync();//获取用户姓名信息列表
                                                                                                                 //遍历关系
                    relations.ForEach(relation =>
                    {
                        var user = userInfos.Where(u => u.Id == relation.TargetId.ToLong()).FirstOrDefault();//获取用户信息
                        if (user != null)
                        {
                            //添加到已读列表
                            messageDetail.ReceiveInfoList.Add(new MessageDetailOutPut.ReceiveInfo
                            {
                                ReceiveUserId = user.Id,
                                ReceiveUserName = user.Name,
                                Read = relation.ExtJson.ToJsonEntity<RelationMsgUser>().Read
                            });
                        }
                        else//用户ID没找到
                        {
                            //添加到已读列表
                            messageDetail.ReceiveInfoList.Add(new MessageDetailOutPut.ReceiveInfo
                            {
                                ReceiveUserId = relation.TargetId.ToLong(),
                                ReceiveUserName = "未知用户",
                                Read = relation.ExtJson.ToJsonEntity<RelationMsgUser>().Read
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
                    await Context.Deleteable<DevRelation>().Where(it => it.Category == CateGoryConst.Relation_MSG_TO_USER && ids.Contains(it.ObjectId)).ExecuteCommandAsync();
                });
                if (!result.IsSuccess)//如果失败了
                {
                    //写日志
                    _logger.LogError(result.ErrorMessage, result.ErrorException);
                    throw Oops.Oh(ErrorCodeEnum.A0002);
                }
            }
        }
    }
}
