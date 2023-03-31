namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {

        //检查ConfigId
        CheckSameConfigId();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {


    }


    /// <summary>
    /// 检查是否有相同的ConfigId
    /// </summary>
    /// <returns></returns>
    private static void CheckSameConfigId()
    {
        var configIdGroup = DbContext.DbConfigs.GroupBy(it => it.ConfigId).ToList();
        foreach (var configId in configIdGroup)
        {
            if (configId.ToList().Count > 1) throw Oops.Oh($"Sqlsugar连接配置ConfigId:{configId.Key}重复了");
        }
    }
}
