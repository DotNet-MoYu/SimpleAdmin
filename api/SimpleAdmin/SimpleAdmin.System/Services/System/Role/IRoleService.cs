namespace SimpleAdmin.System;

/// <summary>
/// 角色服务
/// </summary>
public interface IRoleService : ITransient
{
    /// <summary>
    /// 获取所有角色
    /// </summary>
    /// <returns></returns>
    Task<List<SysRole>> GetListAsync();

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(RoleAddInput input);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(RoleEditInput input);

    /// <summary>
    /// 根据用户ID获取用户角色集合
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<List<SysRole>> GetRoleListByUserId(long userId);

    /// <summary>
    /// 给角色授权权限
    /// </summary>
    /// <param name="input">授权信息</param>
    /// <returns></returns>
    Task GrantPermission(GrantPermissionInput input);

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

    /// <summary>
    /// 获取角色拥有权限
    /// </summary>
    /// <param name="input">角色ID</param>
    /// <returns></returns>
    Task<RoleOwnPermissionOutput> OwnPermission(BaseIdInput input);

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
    Task<List<long>> OwnUser(BaseIdInput input);

    /// <summary>
    /// 分页查询角色
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysRole>> Page(RolePageInput input);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    Task RefreshCache();

    /// <summary>
    /// 获取角色授权权限选择器
    /// </summary>
    /// <param name="input">角色ID</param>
    /// <returns></returns>
    Task<List<string>> RolePermissionTreeSelector(BaseIdInput input);

    /// <summary>
    /// 角色选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SqlSugarPagedList<SysRole>> RoleSelector(RoleSelectorInput input);

    /// <summary>
    /// 根据id集合获取角色集合
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<SysRole>> GetRoleListByIdList(IdListInput input);
}