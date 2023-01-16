namespace SimpleAdmin.System;

/// <summary>
/// 代码生成绑定视图
/// </summary>
public class GenViewModel : GenBasic
{

    /// <summary>
    /// 生成时间
    /// </summary>
    public string GenTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


    /// <summary>
    /// 菜单ID
    /// </summary>
    public long MenuId { get; set; } = YitIdHelper.NextId();

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string MenuCode { get; set; } = RandomHelper.CreateRandomString(10);

    /// <summary>
    /// /菜单路径
    /// </summary>
    public string MenuPath
    {
        get { return $"/{RouteName}/{BusName}"; }
    }

    /// <summary>
    /// /菜单路径
    /// </summary>
    public string MenuComponent
    {
        get { return $"{RouteName}/{BusName}/index"; }
    }


    /// <summary>
    /// 添加按钮ID
    /// </summary>
    public long AddButtonId { get; set; } = YitIdHelper.NextId();

    /// <summary>
    /// 批量删除按钮ID
    /// </summary>
    public long BatchDeleteButtonId { get; set; } = YitIdHelper.NextId();


    /// <summary>
    /// 编辑按钮ID
    /// </summary>
    public long EditButtonId { get; set; } = YitIdHelper.NextId();


    /// <summary>
    /// 删除按钮ID
    /// </summary>
    public long DeleteButtonId { get; set; } = YitIdHelper.NextId();



    /// <summary>
    /// 类名首字母小写
    /// </summary>
    public string ClassNameFirstLower
    {
        get { return ClassName.First().ToString().ToLower() + ClassName.Substring(1); }
    }


}
