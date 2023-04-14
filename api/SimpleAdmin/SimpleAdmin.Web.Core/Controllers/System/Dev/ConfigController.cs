namespace SimpleAdmin.Web.Core;

/// <summary>
/// 系统配置控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统配置")]
[Route("dev/[controller]")]
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
    [AllowAnonymous]
    public async Task<List<DevConfig>> SysBaseList()
    {
        return await _configService.GetListByCategory(CateGoryConst.Config_SYS_BASE);
    }

    /// <summary>
    /// 获取配置列表
    /// </summary>
    /// <param name="category">分类</param>
    /// <returns></returns>
    [HttpGet("list")]
    public async Task<dynamic> List(string category)
    {
        return await _configService.GetListByCategory(category);
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
    public async Task EditBatch([FromBody] List<DevConfig> devConfigs)
    {
        await _configService.EditBatch(devConfigs);
    }
}