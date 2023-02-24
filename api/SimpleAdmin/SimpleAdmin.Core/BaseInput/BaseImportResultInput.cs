using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core
{
    public class BaseImportResultInput<T> where T : class
    {

        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Data { get; set; }

    }
}
