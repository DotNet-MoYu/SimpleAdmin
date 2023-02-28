using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core
{
    /// <summary>
    /// 导入结果输出
    /// </summary>
    public class BaseImportResultOutPut<T> where T : class
    {
        /// <summary>
        /// 是否成功
        /// </summary>

        public bool Success { get; set; } = true;

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 导入数量
        /// </summary>
        public int ImportCount { get; set; }



        /// <summary>
        /// 错误数
        /// </summary>
        public int FailCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>

        public List<T> Data { get; set; } = new List<T>();
    }
}
