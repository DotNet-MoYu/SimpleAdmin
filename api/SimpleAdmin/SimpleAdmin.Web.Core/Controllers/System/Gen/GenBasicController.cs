using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.System.Gen
{
    /// <summary>
    /// 代码生成基础控制器
    /// </summary>
    [ApiDescriptionSettings(Tag = "代码生成基础")]
    [Route("gen/basic")]
    public class GenBasicController : BaseController
    {
        private readonly IGenbasicService _genbasicService;

        public GenBasicController(IGenbasicService genbasicService)
        {
            this._genbasicService = genbasicService;
        }

        /// <summary>
        /// 代码生成基础分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<dynamic> Page([FromQuery] BasePageInput input)
        {
            return await _genbasicService.Page(input);
        }


        /// <summary>
        /// 获取所有表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("tables")]
        public dynamic Tables()
        {
            return _genbasicService.GetTables();
        }

        /// <summary>
        /// 获取项目所有程序集
        /// </summary>
        /// <returns></returns>
        [HttpGet("assemblies")]
        public dynamic GetAssemblies()
        {
            return _genbasicService.GetAssemblies();
        }

        /// <summary>
        /// 添加代码生成器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Description("添加代码生成")]
        [HttpPost("add")]
        public async Task<dynamic> Add([FromBody] GenBasicAddInput input)
        {
            return await _genbasicService.Add(input);
        }

        /// <summary>
        /// 编辑代码生成器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Description("编辑代码生成基础")]
        [HttpPost("edit")]
        public async Task<dynamic> Edit([FromBody] GenBasicEditInput input)
        {
            return await _genbasicService.Edit(input);
        }

        /// <summary>
        /// 删除代码生成配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Description("删除代码生成基础")]
        [HttpPost("delete")]
        public async Task Delete([FromBody] List<BaseIdInput> input)
        {
            await _genbasicService.Delete(input);
        }

        /// <summary>
        /// 代码生成预览
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("previewGen")]
        public async Task<dynamic> PreviewGen([FromQuery] BaseIdInput input)
        {
            return await _genbasicService.PreviewGen(input);
        }
    }
}
