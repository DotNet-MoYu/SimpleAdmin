using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core.Utils
{
    /// <summary>
    /// Sqlsugar表信息
    /// </summary>
    public class SqlSugarTableInfo
    {
        /// <summary>
        /// 所属库 
        ///</summary>
        public virtual string ConfigId { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string TableDescription { get; set; }
    }
}
