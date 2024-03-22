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
/// 模块管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "模块管理")]
[Route("sys/limit/[controller]")]
[SuperAdmin]
public class ModuleController : BaseController
{
    private readonly IModuleService _moduleService;

    public ModuleController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    /// <summary>
    /// 模块分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] ModulePageInput input)
    {
        return await _moduleService.Page(input);
    }

    /// <summary>
    /// 添加模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加模块")]
    public async Task Add([FromBody] ModuleAddInput input)
    {
        await _moduleService.Add(input);
    }

    /// <summary>
    /// 修改模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改模块")]
    public async Task Edit([FromBody] ModuleEditInput input)
    {
        await _moduleService.Edit(input);
    }

    /// <summary>
    /// 删除模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除模块")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _moduleService.Delete(input);
    }

    /// <summary>
    /// 获取模块详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _moduleService.Detail(input);
    }
}
