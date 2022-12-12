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
            if (input.ReceiveUserId > 0)//如果用户ID大于0
            {
                var ids = await Context.Queryable<DevRelation>()//获取关系表中的站内信ID
                    .Where(it => it.Category == CateGoryConst.Relation_MSG_TO_USER && it.TargetId == input.ReceiveUserId.ToString())//根据分类和用户用户ID查询
                    .Select(it => it.ObjectId).ToListAsync();
                input.Ids = ids;
            }
            var query = Context.Queryable<DevMessage>()
                               .WhereIF(input.Ids != null, it => input.Ids.Contains(it.Id))//根据用户ID查询查询
                               .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                               .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Subject.Contains(input.SearchKey) || it.Content.Contains(input.SearchKey))//根据关键字查询
                               .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                               .OrderBy(it => it.CreateTime, OrderByType.Desc);

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
    }
}
