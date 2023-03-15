using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System
{
    /// <summary>
    /// 组织导出
    /// </summary>
    [ExcelExporter(Name = "组织信息", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
    public class SysOrgOutput
    {
        /// <summary>
        /// 名称 
        ///</summary>
        [ExporterHeader(DisplayName = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 上级组织
        ///</summary>
        [ExporterHeader(DisplayName = "上级组织")]
        public string Names { get; set; }

        /// <summary>
        /// 分类 
        ///</summary>
        [ExporterHeader(DisplayName = "分类")]
        public string Category { get; set; }
        /// <summary>
        /// 排序码 
        ///</summary>
        [ExporterHeader(DisplayName = "排序码")]
        public int? SortCode { get; set; }

        /// <summary>
        /// 主管账号 
        ///</summary>
        [ExporterHeader(DisplayName = "主管账号")]
        public string Director { get; set; }

    }
}