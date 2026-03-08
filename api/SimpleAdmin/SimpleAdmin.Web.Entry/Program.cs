// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

Console.WriteLine("源码地址: https://gitee.com/dotnetmoyu/SimpleAdmin");
Console.WriteLine("ts演示地址: http://153.101.199.83:12802/login");
Console.WriteLine("js演示地址: http://153.101.199.83:12801/login");
Console.WriteLine("QQ:531035580");
Console.WriteLine("简单之名，非凡之质——稳定、灵活、高效，实力不需夸耀。");
Console.WriteLine("广告推荐 | 性价比机场：单月12元，128G流量，纯净IP，畅用AI");
Console.WriteLine("邀请链接: https://www.zou666.net/#/register?code=hfkhGLG5");
Console.WriteLine("广告推荐 | 性价比AI中转站：纯血Codex中转，支持GPT-5.4，无需梯子即可访问");
Console.WriteLine("邀请链接: https://teamplus.space/register?invite_code=TP000013LEEI");
Serve.Run(RunOptions.Default.ConfigureBuilder(builder =>
{
    builder.WebHost.UseUrls(builder.Configuration["AppSettings:Urls"]);
}));
