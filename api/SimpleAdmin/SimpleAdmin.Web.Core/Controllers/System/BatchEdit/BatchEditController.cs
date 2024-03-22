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
/// 批量控制器
/// </summary>
[ApiDescriptionSettings(Tag = "批量修改")]
[Route("/sys/batch")]
public class BatchEditController : BaseController
{
    private readonly IBatchEditService _batchEditService;

    public BatchEditController(IBatchEditService updateBatchService)
    {
        _batchEditService = updateBatchService;
    }

    #region Get请求

    /// <summary>
    /// 批量分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] BatchEditPageInput input)
    {
        return await _batchEditService.Page(input);
    }

    /// <summary>
    /// 获取表信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("tables")]
    public dynamic Tables()
    {
        return _batchEditService.GetTables();
    }

    /// <summary>
    /// 获取批量配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("configs")]
    public async Task<dynamic> ConfigList([FromQuery] BaseIdInput input)
    {
        return await _batchEditService.ConfigList(input);
    }

    /// <summary>
    /// 获取批量配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("columns")]
    [IgnoreSuperAdmin]
    public async Task<dynamic> Columns([FromQuery] string code)
    {
        return await _batchEditService.Columns(code);
    }

    #endregion Get请求

    #region Post请求

    /// <summary>
    /// 添加批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加批量更新")]
    public async Task Add([FromBody] BatchEditAddInput input)
    {
        await _batchEditService.Add(input);
    }

    /// <summary>
    /// 配置修改批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("config")]
    [DisplayName("配置批量更新")]
    public async Task Config([FromBody] List<BatchEditConfigInput> input)
    {
        await _batchEditService.Config(input);
    }

    /// <summary>
    /// 删除批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除批量更新")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _batchEditService.Delete(input);
    }

    /// <summary>
    /// 批量更新同步
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sync")]
    [DisplayName("批量更新同步")]
    public async Task Sync([FromBody] BaseIdInput input)
    {
        await _batchEditService.SyncColumns(input);
    }

    #endregion Post请求
}
