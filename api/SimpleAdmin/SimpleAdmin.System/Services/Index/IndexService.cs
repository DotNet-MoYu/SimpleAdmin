namespace SimpleAdmin.System;

/// <summary>
///  <inheritdoc cref="IIndexService"/>
/// </summary>
public class IndexService : DbRepository<SysRelation>, IIndexService
{
    private readonly IRelationService _relationService;

    public IndexService(IRelationService relationService)
    {
        this._relationService = relationService;
    }

    /// <inheritdoc/>
    public async Task<List<ScheduleListOutput>> ScheduleList(ScheduleListInput input)
    {
        var relations = await GetListAsync(it => it.Category == CateGoryConst.Relation_SYS_USER_SCHEDULE_DATA
        && it.ObjectId == UserManager.UserId && it.TargetId == input.ScheduleDate, it => new SysRelation { ExtJson = it.ExtJson, Id = it.Id });//获取当前用户的日程列表
        List<ScheduleListOutput> userSchedules = new List<ScheduleListOutput>();//结果集
        relations.ForEach(it =>
        {
            var extjson = it.ExtJson.ToJsonEntity<RelationUserSchedule>();//转成实体
            var userSchedule = extjson.Adapt<ScheduleListOutput>();//格式化
            userSchedule.Id = it.Id;//赋值ID
            userSchedules.Add(userSchedule);//添加到结果集
        });
        return userSchedules;
    }

    /// <inheritdoc/>
    public async Task AddSchedule(ScheduleAddInput input)
    {
        input.ScheduleUserId = UserManager.UserId;
        input.ScheduleUserName = UserManager.Name;
        //添加日程
        await _relationService.SaveRelation(CateGoryConst.Relation_SYS_USER_SCHEDULE_DATA, UserManager.UserId, input.ScheduleDate, input.ToJson(), false, false);
    }

    /// <inheritdoc/>
    public async Task DeleteSchedule(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        await DeleteAsync(it => ids.Contains(it.Id) && it.ObjectId == UserManager.UserId);
    }
}