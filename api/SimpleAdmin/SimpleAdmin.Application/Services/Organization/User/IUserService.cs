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

/// <summary>
/// 人员管理服务
/// </summary>
public interface IUserService : ITransient
{
    #region 查询

    /// <summary>
    /// 人员选择器
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>人员列表</returns>
    Task<SqlSugarPagedList<UserSelectorOutPut>> Selector(UserSelectorInput input);

    /// <summary>
    /// 用户分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>用户分页列表</returns>
    Task<SqlSugarPagedList<SysUser>> Page(UserPageInput input);

    /// <summary>
    /// 获取用户拥有角色
    /// </summary>
    /// <param name="input">用户ID</param>
    /// <returns></returns>
    Task<List<RoleSelectorOutPut>> OwnRole(BaseIdInput input);

    /// <summary>
    /// 角色选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SqlSugarPagedList<RoleSelectorOutPut>> RoleSelector(RoleSelectorInput input);

    /// <summary>
    /// 人员详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SysUser> Detail(BaseIdInput input);

    #endregion 查询

    #region 添加

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(UserAddInput input);

    #endregion 添加

    #region 编辑

    /// <summary>
    /// 编辑
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(UserEditInput input);

    /// <summary>
    /// 禁用用户
    /// </summary>
    /// <param name="input">用户Id</param>
    /// <returns></returns>
    Task DisableUser(BaseIdInput input);

    /// <summary>
    /// 启用用户
    /// </summary>
    /// <param name="input">用户Id</param>
    /// <returns></returns>
    Task EnableUser(BaseIdInput input);

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

    #endregion 编辑

    #region 删除

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="input">Id列表</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    #endregion 删除

    #region 导入导出

    /// <summary>
    /// 导入预览
    /// </summary>
    /// <param name="input">导入参数</param>
    /// <returns></returns>
    Task<dynamic> Preview(ImportPreviewInput input);

    /// <summary>
    /// 获取导入模板
    /// </summary>
    /// <returns></returns>
    Task<FileStreamResult> Template();

    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<dynamic> Export(UserPageInput input);

    /// <summary>
    /// 导入数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns>导入结果</returns>
    Task<ImportResultOutPut<BizUserImportInput>> Import(ImportResultInput<BizUserImportInput> input);

    /// <summary>
    /// 批量编辑
    /// </summary>
    /// <param name="input">编辑字段信息</param>
    /// <returns></returns>
    Task Edits(BatchEditInput input);

    #endregion 导入导出
}
