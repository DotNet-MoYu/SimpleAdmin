namespace SimpleAdmin.System;

/// <summary>
/// 批量修改配置种子数据
/// </summary>
public class BatchEditSeedData : ISqlSugarEntitySeedData<BatchEdit>
{
    public IEnumerable<BatchEdit> SeedData()
    {
        return SeedDataUtil.GetSeedData<BatchEdit>("seed_batch_edit.json");
    }
}