namespace SimpleAdmin.Web.Core;

/// <summary>
/// 系统字典控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统字典")]
[Route("dev/[controller]")]
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