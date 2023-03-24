namespace SimpleAdmin.Plugin.CodeFirst;

/// <summary>
/// 批量修改配置种子数据
/// </summary>
public class BatchEditSeedData : ISqlSugarEntitySeedData<BatchEdit>
{
    public IEnumerable<BatchEdit> SeedData()
    {
        return SeedDataUtil.GetSeedData<BatchEdit>(SqlsugarConst.DB_Default, "batch_edit.json");
    }

}
