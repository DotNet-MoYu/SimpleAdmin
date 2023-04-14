namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 批量修改字段种子数据
/// </summary>
public class BatchEditConfigSeedData : ISqlSugarEntitySeedData<BatchEditConfig>
{
    public IEnumerable<BatchEditConfig> SeedData()
    {
        return SeedDataUtil.GetSeedData<BatchEditConfig>(SqlsugarConst.DB_Default, "batch_edit_config.json");
    }
}