// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using SimpleAdmin.Core.Utils;

namespace SimpleAdmin.Core;

/// <summary>
/// AppStartup启动类
/// </summary>
[AppStartup(99)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddComponent<LoggingConsoleComponent>();//启动控制台日志格式化组件
        services.AddComponent<LoggingFileComponent>();//启动日志写入文件组件

        // 配置雪花Id算法机器码
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions
        {
            WorkerId = 1// 取值范围0~63,默认1
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
