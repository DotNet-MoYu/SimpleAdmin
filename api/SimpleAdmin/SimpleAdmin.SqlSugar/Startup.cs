using System.Collections;

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    /// <summary>
    /// ConfigureServices中不能解析服务，比如App.GetService()，尤其是不能在ConfigureServices中获取诸如缓存等数据进行初始化，应该在Configure中进行
    /// 服务都还没初始化完成，会导致内存中存在多份 IOC 容器！！
    /// 正确应该在 Configure 中，这个时候服务（IServiceCollection 已经完成 BuildServiceProvider() 操作了
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        //检查ConfigId
        CheckSameConfigId();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //遍历配置
        DbContext.DbConfigs.ForEach(it =>
        {
            var connection = DbContext.Db.GetConnection(it.ConfigId);//获取数据库连接对象
            connection.DbMaintenance.CreateDatabase();//创建数据库,如果存在则不创建
        });
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