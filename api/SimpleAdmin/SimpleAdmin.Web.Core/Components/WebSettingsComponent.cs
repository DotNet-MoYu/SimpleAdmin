// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// Web设置组件
/// 模拟 ConfigureService
/// </summary>
public sealed class WebSettingsComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        // 配置ip2region
        services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content, Path.Combine(App.HostEnvironment.ContentRootPath, "ip2region.xdb")));
        //web设置配置转实体
        services.AddConfigurableOptions<WebSettingsOptions>();

        //禁止在主机启动时通过 App.GetOptions<TOptions> 获取选项，如需获取配置选项理应通过 App.GetConfig<TOptions>("配置节点", true)。
        var appSettings = App.GetConfig<WebSettingsOptions>("WebSettings", true);
        //如果是演示环境,加上操作筛选器,禁止操作数据库
        if (appSettings.EnvPoc)
            services.AddMvcFilter<MyActionFilter>();
    }
}

/// <summary>
/// Web设置组件
/// 模拟 Configure
/// </summary>
public sealed class WebSettingsApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
    }
}
