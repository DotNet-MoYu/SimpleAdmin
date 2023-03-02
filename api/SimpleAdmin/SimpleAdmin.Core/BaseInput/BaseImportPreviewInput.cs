using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core
{

    /// <summary>
    /// 导入预览输入
    /// </summary>
    public class BaseImportPreviewInput
    {
        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile File { get; set; }

    }
}
