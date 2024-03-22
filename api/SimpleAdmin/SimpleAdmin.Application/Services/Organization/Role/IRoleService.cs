// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

public interface IRoleService : ITransient
{
    #region 查询

    /// <summary>
    /// 角色拥有资源
    /// </summary>
    /// <param name="input">角色id</param>
    /// <param name="category">资源类型</param>
    /// <returns>角色拥有资源信息</returns>
    Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input, string category);

    /// <summary>
    /// 获取角色下的用户
    /// </summary>
    /// <param name="input">角色ID</param>
    /// <returns></returns>
    Task<List<UserSelectorOutPut>> OwnUser(BaseIdInput input);

    /// <summary>
    /// 分页查询角色
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysRole>> Page(RolePageInput input);

    /// <summary>
    /// 角色选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SqlSugarPagedList<RoleSelectorOutPut>> RoleSelector(RoleSelectorInput input);

    /// <summary>
    /// 获取角色树
    /// </summary>
    /// <param name="input">角色树</param>
    /// <returns></returns>
    Task<List<RoleTreeOutput>> Tree(RoleTreeInput input);

    /// <summary>
    /// 角色详情
    /// </summary>
    /// <param name="input">角色Id</param>
    /// <returns></returns>
    Task<SysRole> Detail(BaseIdInput input);

    /// <summary>
    /// 资源树选择器
    /// </summary>
    /// <returns></returns>
    Task<List<ResTreeSelector>> ResourceTreeSelector();

    #endregion

    #region 新增

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(RoleAddInput input);

    #endregion

    #region 编辑

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(RoleEditInput input);


    /// <summary>
    /// 给角色授权
    /// </summary>
    /// <param name="input">授权参数</param>
    /// <returns></returns>
    Task GrantResource(GrantResourceInput input);

    /// <summary>
    /// 给角色授权用户
    /// </summary>
    /// <param name="input">授权信息</param>
    /// <returns></returns>
    Task GrantUser(GrantUserInput input);

    #endregion

    #region 删除

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    #endregion
}
