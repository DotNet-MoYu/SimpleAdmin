// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
///  <inheritdoc cref="IIndexService"/>
/// </summary>
public class IndexService : DbRepository<SysRelation>, IIndexService
{
    private readonly IRelationService _relationService;

    public IndexService(IRelationService relationService)
    {
        _relationService = relationService;
    }

    /// <inheritdoc/>
    public async Task<List<ScheduleListOutput>> ScheduleList(ScheduleListInput input)
    {
        var relations = await GetListAsync(
            it => it.Category == CateGoryConst.RELATION_SYS_USER_SCHEDULE_DATA && it.ObjectId == UserManager.UserId
                && it.TargetId == input.ScheduleDate,
            it => new SysRelation { ExtJson = it.ExtJson, Id = it.Id });//获取当前用户的日程列表
        var userSchedules = new List<ScheduleListOutput>();//结果集
        relations.ForEach(it =>
        {
            var extJson = it.ExtJson.ToJsonEntity<RelationUserSchedule>();//转成实体
            var userSchedule = extJson.Adapt<ScheduleListOutput>();//格式化
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
        await _relationService.SaveRelation(CateGoryConst.RELATION_SYS_USER_SCHEDULE_DATA, UserManager.UserId, input.ScheduleDate, input.ToJson(),
            false, false);
    }

    /// <inheritdoc/>
    public async Task DeleteSchedule(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        await DeleteAsync(it => ids.Contains(it.Id) && it.ObjectId == UserManager.UserId);
    }
}
