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
/// 权限按钮控制器
/// </summary>
[ApiDescriptionSettings(Tag = "权限按钮")]
[Route("sys/limit/[controller]")]
[SuperAdmin]
public class ButtonController : BaseController
{
    private readonly IButtonService _buttonService;

    public ButtonController(IButtonService buttonService)
    {
        _buttonService = buttonService;
    }

    /// <summary>
    /// 按钮分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] ButtonPageInput input)
    {
        return await _buttonService.Page(input);
    }

    /// <summary>
    /// 添加按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加按钮")]
    public async Task Add([FromBody] ButtonAddInput input)
    {
        await _buttonService.Add(input);
    }

    /// <summary>
    /// 修改按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改按钮")]
    public async Task Edit([FromBody] ButtonEditInput input)
    {
        await _buttonService.Edit(input);
    }

    /// <summary>
    /// 删除按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除按钮")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _buttonService.Delete(input);
    }

    /// <summary>
    /// 批量新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("batch")]
    [DisplayName("新增按钮")]
    public async Task Batch([FromBody] ButtonAddInput input)
    {
        await _buttonService.AddBatch(input);
    }

    /// <summary>
    /// 获取按钮详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _buttonService.Detail(input);
    }
}
