// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 用户模块事件总线
/// </summary>
public class UserEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly IServiceProvider _services;

    public UserEventSubscriber(IServiceProvider services)
    {
        this._services = services;
    }

    /// <summary>
    /// 根据角色ID列表清除用户缓存
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(EventSubscriberConst.ClearUserCache)]
    public async Task DeleteUserCacheByRoleIds(EventHandlerExecutingContext context)
    {
        var roleIds = (List<long>)context.Source.Payload;//获取角色ID

        // 创建新的作用域
        using var scope = _services.CreateScope();
        // 解析角色服务
        var relationService = scope.ServiceProvider.GetRequiredService<IRelationService>();
        //获取用户和角色关系
        var relations = await relationService.GetRelationListByTargetIdListAndCategory(roleIds.Select(it => it.ToString()).ToList(), CateGoryConst.Relation_SYS_USER_HAS_ROLE);
        if (relations.Count > 0)
        {
            var userIds = relations.Select(it => it.ObjectId).ToList();//用户ID列表
            // 解析用户服务
            var userService = scope.ServiceProvider.GetRequiredService<ISysUserService>();
            //从redis中删除
            userService.DeleteUserFromRedis(userIds);
        }
    }
}