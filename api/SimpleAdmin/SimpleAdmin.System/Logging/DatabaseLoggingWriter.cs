using IPTools.Core;
using Masuit.Tools;
using Newtonsoft.Json;
using UAParser;

namespace SimpleAdmin.System;

/// <summary>
/// 数据库写入器
/// </summary>
public class DatabaseLoggingWriter : IDatabaseLoggingWriter
{
    private readonly SqlSugarScope _db;
    private readonly ILogger<DatabaseLoggingWriter> _logger;

    public DatabaseLoggingWriter(ILogger<DatabaseLoggingWriter> logger)
    {
        _db = DbContext.Db;
        this._logger = logger;
    }
    public async void Write(LogMessage logMsg, bool flush)
    {
        //获取请求json字符串
        var jsonString = logMsg.Context.Get("loggingMonitor").ToString();
        //转成实体
        var loggingMonitor = jsonString.ToJsonEntity<LoggingMonitorJson>();
        //日志时间赋值
        loggingMonitor.LogDateTime = logMsg.LogDateTime;
        //验证失败之类的不记录日志
        if (loggingMonitor.Validation == null)
        {
            var operation = logMsg.Context.Get(LoggingConst.Operation).ToString();//获取操作名称
            var client = (ClientInfo)logMsg.Context.Get(LoggingConst.Client);//获取客户端信息
            var path = logMsg.Context.Get(LoggingConst.Path).ToString();//获取操作名称
            var method = logMsg.Context.Get(LoggingConst.Method).ToString();//获取方法
            //表示访问日志
            if (operation == EventSubscriberConst.LoginB || operation == EventSubscriberConst.LoginOutB)
            {
                //如果没有异常信息
                if (loggingMonitor.Exception == null)
                {
                    await CreateVisitLog(operation, loggingMonitor, client);//添加到访问日志
                }
                else
                {
                    //添加到异常日志
                    await CreateOperationLog(operation, path, loggingMonitor, client);
                }

            }
            else
            {
                //只有定义了Title的POST方法才记录日志
                if (!operation.Contains("/") && method == "POST")
                {
                    //添加到操作日志
                    await CreateOperationLog(operation, path, loggingMonitor, client);
                }

            }
        }

    }


    /// <summary>
    /// 创建访问日志
    /// </summary>
    /// <param name="operation">访问类型</param>
    /// <param name="loggingMonitor">loggingMonitor</param>
    /// <param name="clientInfo">客户端信息</param>
    private async Task CreateVisitLog(string operation, LoggingMonitorJson loggingMonitor, ClientInfo clientInfo)
    {

        var name = "";//用户姓名
        var opAccount = "";//用户账号
        if (operation == EventSubscriberConst.LoginB)
        {
            //如果是登录，用户信息就从返回值里拿
            var result = loggingMonitor.ReturnInformation.Value.ToJson();//返回值转json
            var userInfo = result.ToJsonEntity<SimpleAdminResult<SysUser>>();//格式化成user表
            name = userInfo.Data.Name;//赋值姓名
            opAccount = userInfo.Data.Account;//赋值账号
        }
        else
        {
            //如果是登录出，用户信息就从AuthorizationClaims里拿
            name = loggingMonitor.AuthorizationClaims.Where(it => it.Type == ClaimConst.Name).Select(it => it.Value).FirstOrDefault();
            opAccount = loggingMonitor.AuthorizationClaims.Where(it => it.Type == ClaimConst.Account).Select(it => it.Value).FirstOrDefault();
        }
        //日志表实体
        var devLogVisit = new DevLogVisit
        {
            Name = operation,
            Category = operation == EventSubscriberConst.LoginB ? CateGoryConst.Log_LOGIN : CateGoryConst.Log_LOGOUT,
            ExeStatus = DevLogConst.SUCCESS,
            OpAddress = GetLoginAddress(loggingMonitor.RemoteIPv4),
            OpIp = loggingMonitor.RemoteIPv4,
            OpBrowser = clientInfo.UA.Family + clientInfo.UA.Major,
            OpOs = clientInfo.OS.Family + clientInfo.OS.Major,
            OpTime = loggingMonitor.LogDateTime,
            OpUser = name,
            OpAccount = opAccount,
        };
        await _db.InsertableWithAttr(devLogVisit).IgnoreColumns(true).ExecuteCommandAsync();//入库
    }


    /// <summary>
    /// 创建操作日志
    /// </summary>
    /// <param name="operation">操作名称</param>
    /// <param name="path">请求地址</param>
    /// <param name="loggingMonitor">loggingMonitor</param>
    /// <param name="clientInfo">客户端信息</param>
    /// <returns></returns>
    private async Task CreateOperationLog(string operation, string path, LoggingMonitorJson loggingMonitor, ClientInfo clientInfo)
    {
        //用户名称
        var name = loggingMonitor.AuthorizationClaims?.Where(it => it.Type == ClaimConst.Name).Select(it => it.Value).FirstOrDefault();
        //账号
        var opAccount = loggingMonitor.AuthorizationClaims?.Where(it => it.Type == ClaimConst.Account).Select(it => it.Value).FirstOrDefault();

        //获取参数json字符串，
        var paramJson = loggingMonitor.Parameters == null ? null : loggingMonitor.Parameters[0].Value.ToJsonString();
        //获取结果json字符串
        var resultJson = loggingMonitor.ReturnInformation.Value == null ? null : loggingMonitor.ReturnInformation.Value.ToJsonString();

        //操作日志表实体
        var devLogOperate = new DevLogOperate
        {
            Name = operation,
            Category = CateGoryConst.Log_OPERATE,
            ExeStatus = DevLogConst.SUCCESS,
            OpAddress = GetLoginAddress(loggingMonitor.RemoteIPv4),
            OpIp = loggingMonitor.RemoteIPv4,
            OpBrowser = clientInfo.UA.Family + clientInfo.UA.Major,
            OpOs = clientInfo.OS.Family + clientInfo.OS.Major,
            OpTime = loggingMonitor.LogDateTime,
            OpUser = name,
            OpAccount = opAccount,
            ReqMethod = loggingMonitor.HttpMethod,
            ReqUrl = path,
            ResultJson = resultJson,
            ClassName = loggingMonitor.DisplayName,
            MethodName = loggingMonitor.ActionName,
            ParamJson = paramJson

        };
        //如果异常不为空
        if (loggingMonitor.Exception != null)
        {
            devLogOperate.Category = CateGoryConst.Log_EXCEPTION;//操作类型为异常
            devLogOperate.ExeStatus = DevLogConst.FAIL;//操作状态为失败
            devLogOperate.ExeMessage = loggingMonitor.Exception.Type + ":" + loggingMonitor.Exception.Message + "\n" + loggingMonitor.Exception.StackTrace;
        }
        await _db.InsertableWithAttr(devLogOperate).IgnoreColumns(true).ExecuteCommandAsync();//入库
    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private string GetLoginAddress(string ip)
    {
        var LoginAddress = "未知";
        try
        {
            var ipInfo = IpTool.Search(ip);//解析IP信息
            List<string> LoginAddressList = new List<string>() { ipInfo.Country, ipInfo.Province, ipInfo.City, ipInfo.NetworkOperator };//定义登录地址列表
            LoginAddress = string.Join("|", LoginAddressList.Where(it => it != "0").ToList());//过滤掉0的信息并用|连接成字符串
        }
        catch (global::System.Exception ex)
        {
            _logger.LogError("IP解析错误" + ex, ex);
        }
        return LoginAddress;

    }
}
