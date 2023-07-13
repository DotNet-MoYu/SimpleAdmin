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
    Task Delete(List<BaseIdInput> input, string name = SimpleAdminConst.SysPos);

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