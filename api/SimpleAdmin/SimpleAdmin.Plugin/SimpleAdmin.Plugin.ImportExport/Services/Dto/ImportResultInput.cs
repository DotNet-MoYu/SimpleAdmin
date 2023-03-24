namespace SimpleAdmin.Plugin.ImportExport;

/// <summary>
/// 批量导入结果输入
/// </summary>
/// <typeparam name="T"></typeparam>
public class ImportResultInput<T> where T : class
{

    /// <summary>
    /// 数据
    /// </summary>
    public List<T> Data { get; set; }

}
