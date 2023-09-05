// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 职位服务
/// </summary>
public interface ISysPositionService : ITransient
{
    /// <summary>
    /// 添加职位
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Add(PositionAddInput input, string name = SimpleAdminConst.SysPos);

    /// <summary>
    /// 删除职位
    /// </summary>
    /// <param name="input">id列表</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input, string name = SimpleAdminConst.SysPos);

    /// <summary>
    /// 编辑职位
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <param name="name">名称</param>
    /// <returns></returns>
    Task Edit(PositionEditInput input, string name = SimpleAdminConst.SysPos);

    /// <summary>
    /// 获取职位列表
    /// </summary>
    /// <returns>职位列表</returns>
    Task<List<SysPosition>> GetListAsync();

    /// <summary>
    /// 获取职位信息
    /// </summary>
    /// <param name="id">职位ID</param>
    /// <returns>职位信息</returns>
    Task<SysPosition> GetSysPositionById(long id);

    /// <summary>
    /// 职位分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页列表</returns>
    Task<SqlSugarPagedList<SysPosition>> Page(PositionPageInput input);

    /// <summary>
    /// 职位选择器
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns></returns>
    Task<LinqPagedList<SysPosition>> PositionSelector(PositionSelectorInput input);

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    Task RefreshCache();

    /// <summary>
    /// 根据id集合获取职位集合
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<SysPosition>> GetPositionListByIdList(IdListInput input);
}