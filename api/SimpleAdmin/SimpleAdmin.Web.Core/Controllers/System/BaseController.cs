namespace SimpleAdmin.Web.Core;

/// <summary>
/// 基础控制器
/// </summary>
[Route("sys/[controller]")]
[SuperAdmin]
public class BaseController : IDynamicApiController
{
}