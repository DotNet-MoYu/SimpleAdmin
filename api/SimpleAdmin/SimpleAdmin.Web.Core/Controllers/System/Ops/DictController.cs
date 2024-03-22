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
/// 系统字典控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统字典")]
[Route("sys/ops/[controller]")]
[SuperAdmin]
public class DictController : BaseController
{
    private readonly IDictService _dictService;

    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    /// <summary>
    /// 获取字典树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [IgnoreSuperAdmin]
    public async Task<dynamic> Tree([FromQuery] DictTreeInput input)
    {
        return await _dictService.Tree(input);
    }

    /// <summary>
    /// 字典分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] DictPageInput input)
    {
        return await _dictService.Page(input);
    }

    /// <summary>
    /// 添加字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加字典")]
    public async Task Add([FromBody] DictAddInput input)
    {
        await _dictService.Add(input);
    }

    /// <summary>
    /// 修改字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改字典")]
    public async Task Edit([FromBody] DictEditInput input)
    {
        await _dictService.Edit(input);
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除字典")]
    public async Task Delete([FromBody] DictDeleteInput input)
    {
        await _dictService.Delete(input);
    }
}
