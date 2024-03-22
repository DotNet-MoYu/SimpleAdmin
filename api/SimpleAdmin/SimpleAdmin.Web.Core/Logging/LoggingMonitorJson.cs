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
/// 请求信息格式化
/// </summary>
public class LoggingMonitorJson
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 操作名称
    /// </summary>
    public string DisplayTitle { get; set; }

    /// <summary>
    /// 控制器名
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// 方法名称
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// 类名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 服务端
    /// </summary>
    public string LocalIPv4 { get; set; }

    /// <summary>
    /// 客户端IPV4地址
    /// </summary>
    public string RemoteIPv4 { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string HttpMethod { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    public string RequestUrl { get; set; }

    /// <summary>
    /// 浏览器标识
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 系统名称
    /// </summary>
    public string OsDescription { get; set; }

    /// <summary>
    /// 系统架构
    /// </summary>
    public string OsArchitecture { get; set; }

    /// <summary>
    /// 环境
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// 认证信息
    /// </summary>
    public List<AuthorizationClaims> AuthorizationClaims { get; set; }

    /// <summary>
    /// 参数列表
    /// </summary>
    public List<Parameters> Parameters { get; set; }

    /// <summary>
    /// 返回信息
    /// </summary>
    public ReturnInformation ReturnInformation { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// 验证错误信息
    /// </summary>
    public Validation Validation { get; set; }

    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime LogDateTime { get; set; }
}

/// <summary>
/// 认证信息
/// </summary>
public class AuthorizationClaims
{
    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// 请求参数
/// </summary>
public class Parameters
{
    /// <summary>
    /// 参数名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public object Value { get; set; }
}

/// <summary>
/// 返回信息
/// </summary>
public class ReturnInformation
{
    /// <summary>
    /// 返回值
    /// </summary>
    public ReturnValue Value { get; set; }

    public class ReturnValue
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 额外信息
        /// </summary>
        public object Extras { get; set; }

        /// <summary>
        /// 内如
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }
    }
}

/// <summary>
/// 异常信息
/// </summary>
public class Exception
{
    /// <summary>
    /// 异常类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 异常内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 堆栈信息
    /// </summary>
    public string StackTrace { get; set; }
}

/// <summary>
/// 验证失败信息
/// </summary>
public class Validation
{
    ///// <summary>
    ///// 错误码
    ///// </summary>
    //public string ErrorCode { get; set; }

    ///// <summary>
    ///// 错误码
    ///// </summary>
    //public string OriginErrorCode { get; set; }

    /// <summary>
    /// 错误详情
    /// </summary>
    public string Message { get; set; }
}
