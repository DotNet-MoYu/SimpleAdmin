// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// AppStartup启动类
/// </summary>
[AppStartup(98)]
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
        DbContext.DB_CONFIGS.ForEach(it =>
        {
            var connection = DbContext.DB.GetConnection(it.ConfigId);//获取数据库连接对象
            connection.DbMaintenance.CreateDatabase();//创建数据库,如果存在则不创建
        });
    }

    /// <summary>
    /// 检查是否有相同的ConfigId
    /// </summary>
    /// <returns></returns>
    private static void CheckSameConfigId()
    {
        var configIdGroup = DbContext.DB_CONFIGS.GroupBy(it => it.ConfigId).ToList();
        foreach (var configId in configIdGroup)
        {
            if (configId.ToList().Count > 1) throw Oops.Oh($"SqlSugar连接配置ConfigId:{configId.Key}重复了");
        }
    }
}
