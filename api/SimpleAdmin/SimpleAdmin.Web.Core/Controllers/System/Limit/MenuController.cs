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
/// 菜单管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "菜单管理")]
[Route("sys/limit/[controller]")]
[SuperAdmin]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;
    private readonly IResourceService _resourceService;

    public MenuController(IMenuService menuService, IResourceService resourceService)
    {
        _menuService = menuService;
        _resourceService = resourceService;
    }

    /// <summary>
    /// 模块选择
    /// </summary>
    /// <returns></returns>
    [HttpGet("moduleSelector")]
    public async Task<dynamic> ModuleSelector()
    {
        return await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MODULE);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    public async Task<dynamic> Tree([FromQuery] MenuTreeInput input)
    {
        return await _menuService.Tree(input);
    }

    /// <summary>
    /// 获取菜单树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("menuTreeSelector")]
    public async Task<dynamic> MenuTreeSelector([FromQuery] MenuTreeInput input)
    {
        if (input.Module != null)
        {
            return await _menuService.Tree(input, false);
        }
        else
        {
            return await _menuService.ShortcutTree();
        }
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加菜单")]
    public async Task Add([FromBody] MenuAddInput input)
    {
        await _menuService.Add(input);
    }

    /// <summary>
    /// 编辑菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("编辑菜单")]
    public async Task Edit([FromBody] MenuEditInput input)
    {
        await _menuService.Edit(input);
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _menuService.Detail(input);
        ;
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除菜单")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _menuService.Delete(input);
    }

    /// <summary>
    /// 更改模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("changeModule")]
    [DisplayName("更改模块")]
    public async Task ChangeModule([FromBody] MenuChangeModuleInput input)
    {
        await _menuService.ChangeModule(input);
    }
}
