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
/// 用户服务
/// </summary>
public interface ISysUserService : ITransient
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
    /// <param name="tenantId">租户ID</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetUserByAccount(string account, long? tenantId = null);

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
    /// <param name="tenantId">租户ID</param>
    /// <returns>用户账号名称</returns>
    Task<long> GetIdByPhone(string phone, long? tenantId = null);

    /// <summary>
    /// 用户选择器
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<SqlSugarPagedList<UserSelectorOutPut>> Selector(UserSelectorInput input);

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

    ///  <summary>
    /// 根据用户账号获取用户ID
    ///  </summary>
    ///  <param name="account">用户账号</param>
    ///  <param name="tenantId">租户id</param>
    ///  <returns></returns>
    Task<long> GetIdByAccount(string account, long? tenantId = null);

    /// <summary>
    /// 根据用户手机获取用户信息
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="tenantId">租户Id</param>
    /// <returns>用户信息</returns>
    Task<SysUser> GetUserByPhone(string phone, long? tenantId = null);

    /// <summary>
    /// 获取用户拥有角色
    /// </summary>
    /// <param name="input">用户ID</param>
    /// <returns></returns>
    Task<List<RoleSelectorOutPut>> OwnRole(BaseIdInput input);

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

    /// <summary>
    /// 根据id集合获取用户集合
    /// </summary>
    /// <param name="input">Id集合</param>
    /// <returns></returns>
    Task<List<UserSelectorOutPut>> GetUserListByIdList(IdListInput input);

    /// <summary>
    /// 用户详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SysUser> Detail(BaseIdInput input);

    /// <summary>
    /// 获取用户头像
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <returns>base64头像</returns>
    Task<string> GetUserAvatar(long userId);

    #endregion 查询

    #region 数据范围相关

    /// <summary>
    /// 获取当前API用户的数据范围
    /// null:代表拥有全部数据权限
    /// [xx,xx]:代表拥有部分机构的权限
    /// []：代表仅自己权限
    /// </summary>
    /// <returns>机构列表</returns>
    Task<List<long>?> GetLoginUserApiDataScope();

    /// <summary>
    /// 检查用户是否有机构的数据权限
    /// </summary>
    /// <param name="orgId">机构id</param>
    /// <param name="createUerId">创建者id</param>
    /// <param name="errMsg">错误提示:不为空则直接抛出异常</param>
    /// <returns>是否有权限</returns>
    Task<bool> CheckApiDataScope(long? orgId, long? createUerId, string errMsg = "");

    /// <summary>
    /// 检查用户是否有机构的数据权限
    /// </summary>
    /// <param name="orgIds">机构id列表</param>
    /// <param name="createUerIds">创建者id列表</param>
    /// <param name="errMsg">错误提示:不为空则直接抛出异常</param>
    /// <returns></returns>
    Task<bool> CheckApiDataScope(List<long> orgIds, List<long> createUerIds, string errMsg = "");

    #endregion

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
    Task Delete(BaseIdListInput input);

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
