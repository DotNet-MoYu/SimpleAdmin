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
/// 站内信控制器
/// </summary>
[ApiDescriptionSettings(Tag = "站内信")]
[Route("sys/dev/[controller]")]
public class MessageController : BaseController
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] MessagePageInput input)
    {
        return await _messageService.Page(input);
    }

    /// <summary>
    /// 发送站内信
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("send")]
    [DisplayName("发送站内信")]
    public async Task Send([FromBody] MessageSendInput input)
    {
        await _messageService.Send(input);
    }

    /// <summary>
    /// 消息详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _messageService.Detail(input);
    }

    /// <summary>
    /// 删除站内信
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除站内信")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _messageService.Delete(input);
    }
}
