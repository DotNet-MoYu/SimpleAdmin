// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using Masuit.Tools;
using Microsoft.Extensions.Logging;
using NewLife.Serialization;
using SqlSugar;
using System.Globalization;
using UAParser;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 数据库写入器
/// </summary>
public class DatabaseLoggingWriter : IDatabaseLoggingWriter
{
    private readonly ISearcher _searcher;
    private readonly SqlSugarScope _db;
    private readonly ILogger<DatabaseLoggingWriter> _logger;

    public DatabaseLoggingWriter(ILogger<DatabaseLoggingWriter> logger, ISearcher searcher)
    {
        _db = DbContext.DB;
        _logger = logger;
        _searcher = searcher;
    }

    public async Task WriteAsync(LogMessage logMsg, bool flush)
    {
        //获取请求json字符串
        var jsonString = logMsg.Context.Get("loggingMonitor").ToString();
        //转成实体
        var loggingMonitor = jsonString.ToJsonEntity<LoggingMonitorJson>();
        //日志时间赋值
        loggingMonitor.LogDateTime = logMsg.LogDateTime;
        // loggingMonitor.ReturnInformation.Value
        //验证失败和没有DisplayTitle之类的不记录日志
        if (loggingMonitor.Validation == null && loggingMonitor.DisplayTitle != null)
        {
            //获取操作名称
            var operation = loggingMonitor.DisplayTitle;
            var client = (ClientInfo)logMsg.Context.Get(LoggingConst.CLIENT);//获取客户端信息
            var path = logMsg.Context.Get(LoggingConst.PATH).ToString();//获取操作名称
            var method = logMsg.Context.Get(LoggingConst.METHOD).ToString();//获取方法
            //表示访问日志
            if (operation == EventSubscriberConst.LOGIN_B || operation == EventSubscriberConst.LOGIN_OUT_B)
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
        string name;//用户姓名
        string opAccount;//用户账号
        if (operation == EventSubscriberConst.LOGIN_B)
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
            name = loggingMonitor.AuthorizationClaims.Where(it => it.Type == ClaimConst.NAME).Select(it => it.Value).FirstOrDefault();
            opAccount = loggingMonitor.AuthorizationClaims.Where(it => it.Type == ClaimConst.ACCOUNT).Select(it => it.Value).FirstOrDefault();
        }
        //日志表实体
        var devLogVisit = new SysLogVisit
        {
            Name = operation,
            Category = operation == EventSubscriberConst.LOGIN_B ? CateGoryConst.LOG_LOGIN : CateGoryConst.LOG_LOGOUT,
            ExeStatus = SysLogConst.SUCCESS,
            OpAddress = GetLoginAddress(loggingMonitor.RemoteIPv4),
            OpIp = loggingMonitor.RemoteIPv4,
            OpBrowser = clientInfo.UA.Family + clientInfo.UA.Major,
            OpOs = clientInfo.OS.Family + clientInfo.OS.Major,
            OpTime = loggingMonitor.LogDateTime,
            OpUser = name,
            OpAccount = opAccount
        };
        await _db.CopyNew().InsertableWithAttr(devLogVisit).IgnoreColumns(true).SplitTable().ExecuteCommandAsync();//入库
    }

    /// <summary>
    /// 创建操作日志
    /// </summary>
    /// <param name="operation">操作名称</param>
    /// <param name="path">请求地址</param>
    /// <param name="loggingMonitor">loggingMonitor</param>
    /// <param name="clientInfo">客户端信息</param>
    /// <returns></returns>
    private async Task CreateOperationLog(string operation, string path, LoggingMonitorJson loggingMonitor,
        ClientInfo clientInfo)
    {
        //用户名称
        var name = loggingMonitor.AuthorizationClaims?.Where(it => it.Type == ClaimConst.NAME).Select(it => it.Value).FirstOrDefault();
        //账号
        var opAccount = loggingMonitor.AuthorizationClaims?.Where(it => it.Type == ClaimConst.ACCOUNT).Select(it => it.Value).FirstOrDefault();

        //获取参数json字符串，
        var paramJson = loggingMonitor.Parameters == null || loggingMonitor.Parameters.Count == 0
            ? null
            : loggingMonitor.Parameters[0].Value.ToJsonString();

        //获取结果json字符串
        var resultJson = string.Empty;
        if (loggingMonitor.ReturnInformation != null)//如果有返回值
        {
            if (loggingMonitor.ReturnInformation.Value != null)//如果返回值不为空
            {
                var time = loggingMonitor.ReturnInformation.Value.Time != null
                    ? DateTime.Parse(loggingMonitor.ReturnInformation.Value.Time)
                    : DateTime.Now;//转成时间
                loggingMonitor.ReturnInformation.Value.Time = time.ToString(CultureInfo.CurrentCulture);//转成字符串
                resultJson = loggingMonitor.ReturnInformation.Value.ToJsonString();
            }
        }

        //操作日志表实体
        var devLogOperate = new SysLogOperate
        {
            Name = operation,
            Category = CateGoryConst.LOG_OPERATE,
            ExeStatus = SysLogConst.SUCCESS,
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
            devLogOperate.Category = CateGoryConst.LOG_EXCEPTION;//操作类型为异常
            devLogOperate.ExeStatus = SysLogConst.FAIL;//操作状态为失败
            devLogOperate.ExeMessage = loggingMonitor.Exception.Type + ":" + loggingMonitor.Exception.Message + "\n"
                + loggingMonitor.Exception.StackTrace;
        }
        await _db.CopyNew().InsertableWithAttr(devLogOperate).IgnoreColumns(true).SplitTable().ExecuteCommandAsync();//入库
    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private string GetLoginAddress(string ip)
    {
        var loginAddress = "未知";
        try
        {
            var ipInfo = _searcher.Search(ip);//解析ip
            loginAddress = ipInfo?.Replace("0|", "");//去掉前面的0|
        }
        catch (global::System.Exception ex)
        {
            _logger.LogError("IP解析错误" + ex, ex);
        }
        return loginAddress;
    }
}
