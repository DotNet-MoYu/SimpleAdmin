namespace SimpleAdmin.Application;

/// <summary>
/// 岗位管理
/// </summary>
public interface IPositionService : ITransient
{
    /// <summary>
    /// 添加岗位
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(PositionAddInput input);

    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="input">id列表</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 编辑岗位
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(PositionEditInput input);

    /// <summary>
    /// 岗位分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页列表</returns>
    Task<SqlSugarPagedList<SysPosition>> Page(PositionPageInput input);

    /// <summary>
    /// 岗位选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<LinqPagedList<SysPosition>> PositionSelector(PositionSelectorInput input);

    /// <summary>
    /// 岗位详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SysPosition> Detail(BaseIdInput input);
}