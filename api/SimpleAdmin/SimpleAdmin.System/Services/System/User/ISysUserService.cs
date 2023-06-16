namespace SimpleAdmin.System;

/// <summary>
/// 用户服务
/// </summary>
public partial interface ISysUserService : ITransient
{
    #region 查询

    /// <summary>
    /// 根据用户ID获取按钮ID集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<string>> GetButtonCodeList(long userId);

    /// <summary>
    /// 根据账号获取用户信息
    /// </summary>
    /// <param name="account">用户名</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetUserByAccount(string account);

    /// <summary>
    /// 根据用户ID和机构ID获取角色权限
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orgId"></param>
    /// <returns></returns>
    Task<List<DataScope>> GetPermissionListByUserId(long userId, long orgId);

    /// <summary>
    /// 根据手机号获取用户账号
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns>用户账号名称</returns>
    Task<long> GetIdByPhone(string phone);

    /// <summary>
    /// 用户选择器
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<List<UserSelectorOutPut>> UserSelector(UserSelectorInput input);

    /// <summary>
    /// 用户分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>用户分页列表</returns>
    Task<SqlSugarPagedList<SysUser>> Page(UserPageInput input);

    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="input">查询</param>
    /// <returns></returns>
    Task<List<SysUser>> List(UserPageInput input);

    /// <summary>
    /// 根据用户Id获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetUserById(long userId);

    /// <summary>
    /// 根据用户Id获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <typeparam name="T">转换的实体</typeparam>
    /// <returns></returns>
    Task<T> GetUserById<T>(long userId);

    /// <summary>
    ///根据用户账号获取用户ID
    /// </summary>
    /// <param name="account">用户账号</param>
    /// <returns></returns>
    Task<long> GetIdByAccount(string account);

    /// <summary>
    /// 根据用户手机获取用户信息
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetUserByPhone(string phone);

    /// <summary>
    /// 获取用户拥有角色
    /// </summary>
    /// <param name="input">用户ID</param>
    /// <returns></returns>
    Task<List<long>> OwnRole(BaseIdInput input);

    /// <summary>
    /// 获取当前API用户的数据范围
    /// </summary>
    /// <returns>机构列表</returns>
    Task<List<long>?> GetLoginUserApiDataScope();

    /// <summary>
    /// 获取用户拥有的资源
    /// </summary>
    /// <param name="input">用户id</param>
    /// <returns>资源列表</returns>
    Task<RoleOwnResourceOutput> OwnResource(BaseIdInput input);

    /// <summary>
    /// 获取用户拥有的权限
    /// </summary>
    /// <param name="input">用户id</param>
    /// <returns>权限列表</returns>
    Task<RoleOwnPermissionOutput> OwnPermission(BaseIdInput input);

    /// <summary>
    /// 用户权限树选择
    /// </summary>
    /// <param name="input">用户id</param>
    /// <returns>权限列表</returns>
    Task<List<string>> UserPermissionTreeSelector(BaseIdInput input);

    #endregion 查询

    #region 新增

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(UserAddInput input);

    #endregion 新增

    #region 编辑

    /// <summary>
    /// 编辑
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(UserEditInput input);

    /// <summary>
    /// 启用用户
    /// </summary>
    /// <param name="input">用户Id</param>
    /// <returns></returns>
    Task EnableUser(BaseIdInput input);

    /// <summary>
    /// 禁用用户
    /// </summary>
    /// <param name="input">用户Id</param>
    /// <returns></returns>
    Task DisableUser(BaseIdInput input);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="input">用户Id</param>
    /// <returns></returns>
    Task ResetPassword(BaseIdInput input);

    /// <summary>
    /// 给用户授权角色
    /// </summary>
    /// <param name="input">授权参数</param>
    /// <returns></returns>
    Task GrantRole(UserGrantRoleInput input);

    /// <summary>
    /// 给用户授权资源
    /// </summary>
    /// <param name="input">授权参数</param>
    /// <returns></returns>
    Task GrantResource(UserGrantResourceInput input);

    /// <summary>
    /// 给用户授权权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task GrantPermission(GrantPermissionInput input);

    /// <summary>
    /// 设置用户默认值
    /// </summary>
    /// <param name="sysUsers"></param>
    /// <returns></returns>
    Task SetUserDefault(List<SysUser> sysUsers);

    /// <summary>
    /// 批量编辑
    /// </summary>
    /// <param name="input">批量编辑信息</param>
    /// <returns></returns>
    Task Edits(BatchEditInput input);

    #endregion 编辑

    #region 删除

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="input">Id列表</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 从redis中删除用户信息
    /// </summary>
    /// <param name="ids">用户ID列表</param>
    void DeleteUserFromRedis(List<long> ids);

    /// <summary>
    /// 从redis中删除用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    void DeleteUserFromRedis(long userId);

    #endregion 删除

    #region 导入导出

    /// <summary>
    /// 导出用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<FileStreamResult> Export(UserPageInput input);

    /// <summary>
    /// 导入模板下载
    /// </summary>
    /// <returns>模板</returns>
    Task<FileStreamResult> Template();

    /// <summary>
    /// 导入预览
    /// </summary>
    /// <param name="input">预览参数</param>
    /// <returns>预览结果</returns>
    Task<ImportPreviewOutput<SysUserImportInput>> Preview(ImportPreviewInput input);

    /// <summary>
    /// 用户导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ImportResultOutPut<SysUserImportInput>> Import(ImportResultInput<SysUserImportInput> input);

    /// <summary>
    /// 检查导入数据
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="clearError">是否初始化错误</param>
    /// <returns></returns>
    Task<List<T>> CheckImport<T>(List<T> data, bool clearError = false) where T : SysUserImportInput;

    #endregion 导入导出
}