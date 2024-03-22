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
    Task<List<PositionSelectorOutput>> Selector(PositionSelectorInput input);

    /// <summary>
    /// 岗位详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<SysPosition> Detail(BaseIdInput input);

    /// <summary>
    /// 岗位树形结构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<PositionTreeOutput>> Tree(PositionTreeInput input);
}
