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
/// Web启动项配置
/// </summary>
[AppStartup(99)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //启动LoggingMonitor操作日志写入数据库组件
        services.AddComponent<LoggingMonitorComponent>();

        //认证组件
        services.AddComponent<AuthComponent>();
        //启动Web设置ConfigureServices组件
        services.AddComponent<WebSettingsComponent>();
        //gip压缩
        services.AddComponent<GzipCompressionComponent>();
        //定时任务
        //services.AddSchedule();
        //添加控制器相关
        services.AddControllers().AddNewtonsoftJson(options => //配置json
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();// 首字母小写（驼峰样式）
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";// 时间格式化
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;// 忽略循环引用
            }).AddInjectWithUnifyResult<SimpleAdminResultProvider>()//配置统一返回模型
            ;

        //Nginx代理的话获取真实IP
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //新增如下两行
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //启动Web设置Configure组件
        app.UseComponent<WebSettingsApplicationComponent>(env);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //启用gzip压缩
            app.UseResponseCompression();
        }
        // 启用HTTPS
        app.UseHttpsRedirection();

        // 添加状态码拦截中间件
        app.UseUnifyResultStatusCodes();

        app.UseStaticFiles();//静态文件访问配置

        //静态文件访问配置,暂时不开启
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     ServeUnknownFileTypes = true,
        //     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),//wwwroot相当于真实目录
        //     OnPrepareResponse = c =>
        //     {
        //         c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        //         c.Context.Response.Headers.Append("Cache-Control", "public, max-age=604800");
        //     },
        //     RequestPath = new PathString("/files")//src相当于别名，为了安全
        // });
        app.UseRouting();//路由配置
        app.UseCorsAccessor();//跨域配置
        app.UseWebStatus();//网站开启状态
        app.UseAuthentication();//认证
        app.UseAuthorization();//授权
        app.UseInject(string.Empty);
        app.UseForwardedHeaders();//Nginx代理的话获取真实IP
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
