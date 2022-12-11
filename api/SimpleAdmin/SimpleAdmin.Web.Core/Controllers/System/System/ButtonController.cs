using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.Systems.System;

/// <summary>
/// 权限按钮控制器
/// </summary>
[ApiDescriptionSettings(Tag = "权限按钮")]
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
    [Description("添加按钮")]
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
    [Description("修改按钮")]
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
    [Description("删除按钮")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _buttonService.Delete(input);
    }

    /// <summary>
    /// 批量新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("batch")]
    [Description("新增按钮")]
    public async Task Batch([FromBody] ButtonAddInput input)
    {
        await _buttonService.AddBatch(input);
    }
}
