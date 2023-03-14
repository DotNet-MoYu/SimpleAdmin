using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System.SeedData
{
    /// <summary>
    /// 批量修改字段种子数据
    /// </summary>
    public class BatchEditConfigSeedData : ISqlSugarEntitySeedData<BatchEditConfig>
    {
        public IEnumerable<BatchEditConfig> SeedData()
        {
            return SeedDataUtil.GetSeedData<BatchEditConfig>("batch_edit_config.json");
        }

    }
}
