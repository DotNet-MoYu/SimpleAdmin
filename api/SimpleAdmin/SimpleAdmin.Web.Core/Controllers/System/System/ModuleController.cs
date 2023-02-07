namespace SimpleAdmin.Web.Core;
/// <summary>
/// 模块管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "模块管理")]
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
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _moduleService.Delete(input);
    }


}
