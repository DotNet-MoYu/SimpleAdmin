namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// SYS_USER_WORKBENCH_DATA
/// 用户工作台扩展
/// </summary>
public class RelationUserWorkBench
{

    /// <summary>
    /// 快捷方式列表
    /// </summary>
    public List<WorkBenchShortcut> Shortcut { get; set; }

}

/// <summary>
/// 工作台快捷方式信息
/// </summary>
public class WorkBenchShortcut
{
    /// <summary>
    /// id
    /// </summary>
    public long Id { get; set; }


    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }


    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }


    /// <summary>
    /// 路由地址
    /// </summary>
    public string Path { get; set; }



}
