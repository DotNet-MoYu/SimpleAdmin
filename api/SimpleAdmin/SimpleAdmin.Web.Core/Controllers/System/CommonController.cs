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
/// 系统通用控制器,一般是一些公共的接口
/// </summary>
[Route("sys")]
public class CommonController : BaseController
{
    private readonly ISysOrgService _sysOrgService;
    private readonly IConfigService _configService;

    public CommonController(IConfigService configService, ISysOrgService sysOrgService)
    {
        _sysOrgService = sysOrgService;
        _configService = configService;
    }


    /// <summary>
    /// 获取系统基础配置
    /// </summary>
    /// <returns></returns>
    [HttpGet("ico")]
    [AllowAnonymous]
    [NonUnify]
    public async Task<dynamic> Ico()
    {
        return await _configService.GetIco();
    }

    /// <summary>
    /// 获取系统基础配置
    /// </summary>
    /// <returns></returns>
    [HttpGet("sysInfo")]
    [AllowAnonymous]
    public async Task<dynamic> SysInfo()
    {
        var sysBase = await _configService.GetConfigsByCategory(CateGoryConst.CONFIG_SYS_BASE);
        //排除掉一些不需要的配置
        var configKeys = new List<string>()
        {
            SysConfigConst.SYS_ICO,
            SysConfigConst.SYS_WEB_STATUS,
            SysConfigConst.SYS_WEB_CLOSE_PROMPT,
            SysConfigConst.SYS_DEFAULT_WORKBENCH_DATA
        };
        sysBase = sysBase.Where(x => !configKeys.Contains(x.ConfigKey)).ToList();
        return sysBase;
    }

    /// <summary>
    /// 获取系统基础配置
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginPolicy")]
    [AllowAnonymous]
    public async Task<dynamic> LoginPolicy()
    {
        var loginPolicy = await _configService.GetConfigsByCategory(CateGoryConst.CONFIG_LOGIN_POLICY);//登录策略
        return loginPolicy;
    }

    /// <summary>
    /// 获取租户列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("tenantList")]
    [AllowAnonymous]
    public async Task<dynamic> TenantList()
    {
        return await _sysOrgService.GetTenantList();
    }
}
