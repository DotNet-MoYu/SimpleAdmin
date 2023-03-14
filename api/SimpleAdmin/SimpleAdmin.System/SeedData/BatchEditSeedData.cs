using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System.SeedData
{
    /// <summary>
    /// 批量修改配置种子数据
    /// </summary>
    public class BatchEditSeedData : ISqlSugarEntitySeedData<BatchEdit>
    {
        public IEnumerable<BatchEdit> SeedData()
        {
            return SeedDataUtil.GetSeedData<BatchEdit>("batch_edit.json");
        }

    }
}
