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
/// 文件回收站
/// </summary>
[ApiDescriptionSettings("Application", Tag = "文件回收站")]
[Route("biz/document/trash")]
[RolePermission]
public class TrashController : IDynamicApiController
{
    private readonly ITrashService _trashService;

    public TrashController(ITrashService trashService)
    {
        _trashService = trashService;
    }

    [HttpGet("page")]
    [DisplayName("回收站分页查询")]
    public async Task<dynamic> Page([FromQuery] DocumentPageInput input) => await _trashService.Page(input);

    [HttpPost("recover")]
    [DisplayName("恢复文件")]
    public async Task Recover([FromBody] BaseIdListInput input) => await _trashService.Recover(input);

    [HttpPost("batchRecover")]
    [DisplayName("批量恢复文件")]
    public async Task BatchRecover([FromBody] BaseIdListInput input) => await _trashService.Recover(input);

    [HttpPost("deletePermanent")]
    [DisplayName("永久删除文件")]
    public async Task DeletePermanent([FromBody] BaseIdListInput input) => await _trashService.DeletePermanent(input);

    [HttpPost("batchDeletePermanent")]
    [DisplayName("批量永久删除文件")]
    public async Task BatchDeletePermanent([FromBody] BaseIdListInput input) => await _trashService.DeletePermanent(input);

    [HttpPost("empty")]
    [DisplayName("清空回收站")]
    public async Task Empty() => await _trashService.Empty();
}
