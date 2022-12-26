using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 启动项配置
/// </summary>
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
        //事件总线
        services.AddEventBus();
        //添加控制器相关
        services.AddControllers()
                     .AddInjectWithUnifyResult<SimpleAdminResultProvider>()//配置统一返回模型
                     .AddNewtonsoftJson(options =>//配置json
                     {
                         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 首字母小写（驼峰样式）
                         options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // 时间格式化                                          
                         options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // 忽略循环引用
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

        app.UseRouting();

        //app.UseCorsAccessor();

        app.UseCorsAccessor(builder =>
        builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
      );

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            //通过 App.GetOptions<TOptions> 获取选项
            var webSettings = App.GetOptions<WebSettingsOptions>();
            if (!webSettings.UseMqtt)
            {
                // 注册集线器
                endpoints.MapHubs();
            }
            endpoints.MapControllers();
        });
    }
}