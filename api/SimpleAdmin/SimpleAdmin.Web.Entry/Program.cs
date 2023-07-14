using System.Drawing;

Console.WriteLine(@"源码地址: https://gitee.com/zxzyjs/SimpleAdmin");
Console.WriteLine(@"演示地址: http://153.101.199.83:12802/login");
Console.WriteLine(@"QQ:531035580");
Console.WriteLine("没有花里胡哨，只有简单、稳定、灵活、高效");
Colorful.Console.WriteAsciiAlternating("SimpleAdmin", new Colorful.FrequencyBasedColorAlternator(3, Color.Yellow, Color.GreenYellow));
Serve.Run(RunOptions.Default.ConfigureBuilder(builder =>
{
    builder.WebHost.UseUrls(builder.Configuration["AppSettings:Urls"]);
}));