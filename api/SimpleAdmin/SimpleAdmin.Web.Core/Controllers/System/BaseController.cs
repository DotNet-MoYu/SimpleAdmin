using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.System;

/// <summary>
/// 基础控制器
/// </summary>
[Route("sys/[controller]")]
[SuperAdmin]
public class BaseController : IDynamicApiController
{

}
