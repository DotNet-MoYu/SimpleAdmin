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
/// 系统配置控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统配置")]
[Route("sys/ops/[controller]")]
[SuperAdmin]
public class ConfigController : BaseController
{
    private readonly IConfigService _configService;//系统配置服务

    public ConfigController(IConfigService configService)
    {
        _configService = configService;
    }


    /// <summary>
    /// 获取系统基础配置
    /// </summary>
    /// <returns></returns>
    [HttpGet("sysBaseList")]
    public async Task<dynamic> SysBaseList()
    {
        var sysBase = await _configService.GetConfigsByCategory(CateGoryConst.CONFIG_SYS_BASE);//系统基础
        var loginPolicy = await _configService.GetConfigsByCategory(CateGoryConst.CONFIG_LOGIN_POLICY);//登录策略
        sysBase.AddRange(loginPolicy);//合并登录策略
        return sysBase;
    }

    /// <summary>
    /// 获取系统配置列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<dynamic> List()
    {
        return await _configService.GetSysConfigList();
    }

    /// <summary>
    /// 配置分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] ConfigPageInput input)
    {
        return await _configService.Page(input);
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加配置")]
    public async Task Add([FromBody] ConfigAddInput input)
    {
        await _configService.Add(input);
    }

    /// <summary>
    /// 修改配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改配置")]
    public async Task Edit([FromBody] ConfigEditInput input)
    {
        await _configService.Edit(input);
    }

    /// <summary>
    /// 删除配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除配置")]
    public async Task Delete([FromBody] ConfigDeleteInput input)
    {
        await _configService.Delete(input);
    }

    /// <summary>
    /// 配置批量更新
    /// </summary>
    /// <returns></returns>
    [HttpPost("editBatch")]
    [DisplayName("修改配置")]
    public async Task EditBatch([FromBody] List<SysConfig> devConfigs)
    {
        await _configService.EditBatch(devConfigs);
    }
}
