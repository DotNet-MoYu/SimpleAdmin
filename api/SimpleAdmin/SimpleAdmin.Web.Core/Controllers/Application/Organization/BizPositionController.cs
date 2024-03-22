// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 岗位管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "岗位管理")]
[Route("biz/organization/position")]
[RolePermission]
public class BizPositionController
{
    private readonly IPositionService _positionService;

    public BizPositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    #region Get请求

    /// <summary>
    /// 获取职位树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [DisplayName("获取职位树")]
    public async Task<dynamic> Tree([FromQuery] PositionTreeInput input)
    {
        return await _positionService.Tree(input);
    }

    /// <summary>
    /// 岗位分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [DisplayName("岗位分页查询")]
    public async Task<dynamic> Page([FromQuery] PositionPageInput input)
    {
        return await _positionService.Page(input);
    }

    /// <summary>
    /// 岗位详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [DisplayName("岗位详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _positionService.Detail(input);
    }

    /// <summary>
    /// 岗位选择器
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("selector")]
    [DisplayName("岗位选择器")]
    public async Task<dynamic> Selector([FromQuery] PositionSelectorInput input)
    {
        return await _positionService.Selector(input);
    }

    #endregion

    #region Post请求

    /// <summary>
    /// 添加岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加岗位")]
    public async Task Add([FromBody] PositionAddInput input)
    {
        await _positionService.Add(input);
    }

    /// <summary>
    /// 修改岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改岗位")]
    public async Task Edit([FromBody] PositionEditInput input)
    {
        await _positionService.Edit(input);
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除岗位")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _positionService.Delete(input);
    }

    #endregion
}
