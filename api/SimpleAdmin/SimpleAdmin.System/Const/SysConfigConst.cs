// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 系统配置常量
/// </summary>
public class SysConfigConst
{
    #region MyRegion

    /// <summary>
    ///  系统名称
    /// </summary>
    public const string SYS_NAME = "SYS_NAME";


    /// <summary>
    /// ico图标
    /// </summary>
    public const string SYS_ICO = "SYS_ICO";

    /// <summary>
    /// 网站开启访问
    /// </summary>
    public const string SYS_WEB_STATUS = "SYS_WEB_STATUS";


    /// <summary>
    /// 网站关闭提示
    /// </summary>
    public const string SYS_WEB_CLOSE_PROMPT = "SYS_WEB_CLOSE_PROMPT";

    /// <summary>
    ///  系统logo
    /// </summary>
    public const string SYS_LOGO = "SYS_LOGO";

    /// <summary>
    ///  系统版本
    /// </summary>
    public const string SYS_VERSION = "SYS_VERSION";

    /// <summary>
    ///  多租户开关
    /// </summary>
    public const string SYS_TENANT_OPTIONS = "SYS_TENANT_OPTIONS";


    /// <summary>
    /// 系统默认工作台
    /// </summary>
    public const string SYS_DEFAULT_WORKBENCH_DATA = "SYS_DEFAULT_WORKBENCH_DATA";

    #endregion

    #region 登录策略

    /// <summary>
    /// 登录验证码开关
    /// </summary>
    public const string LOGIN_CAPTCHA_OPEN = "LOGIN_CAPTCHA_OPEN";

    /// <summary>
    /// 登录验证码开关
    /// </summary>
    public const string LOGIN_CAPTCHA_TYPE = "LOGIN_CAPTCHA_TYPE";

    /// <summary>
    /// 单用户登录开关
    /// </summary>
    public const string LOGIN_SINGLE_OPEN = "LOGIN_SINGLE_OPEN";

    /// <summary>
    ///  登录错误锁定时长
    /// </summary>
    public const string LOGIN_ERROR_LOCK = "LOGIN_ERROR_LOCK";

    /// <summary>
    ///  登录错误锁定时长
    /// </summary>
    public const string LOGIN_ERROR_RESET_TIME = "LOGIN_ERROR_RESET_TIME";

    /// <summary>
    /// 登录错误次数
    /// </summary>
    public const string LOGIN_ERROR_COUNT = "LOGIN_ERROR_COUNT";

    #endregion 登录策略

    #region 密码策略

    /// <summary>
    /// 默认用户密码
    /// </summary>
    public const string PWD_DEFAULT_PASSWORD = "PWD_DEFAULT_PASSWORD";

    /// <summary>
    /// 密码定期提醒更新
    /// </summary>
    public const string PWD_REMIND = "PWD_REMIND";

    /// <summary>
    /// 密码定期提醒更新时间
    /// </summary>
    public const string PWD_REMIND_DAY = "PWD_REMIND_DAY";

    /// <summary>
    /// 修改初始密码提醒
    /// </summary>
    public const string PWD_UPDATE_DEFAULT = "PWD_UPDATE_DEFAULT";

    /// <summary>
    /// 密码最小长度
    /// </summary>
    public const string PWD_MIN_LENGTH = "PWD_MIN_LENGTH";

    /// <summary>
    /// 包含数字
    /// </summary>
    public const string PWD_CONTAIN_NUM = "PWD_CONTAIN_NUM";

    /// <summary>
    /// 包含小写字母
    /// </summary>
    public const string PWD_CONTAIN_LOWER = "PWD_CONTAIN_LOWER";

    /// <summary>
    /// 包含大写字母
    /// </summary>
    public const string PWD_CONTAIN_UPPER = "PWD_CONTAIN_UPPER";

    /// <summary>
    /// 包含特殊字符
    /// </summary>
    public const string PWD_CONTAIN_CHARACTER = "PWD_CONTAIN_UPPER";

    #endregion 密码策略

    #region 存储引擎

    /// <summary>
    /// windows系统本地目录
    /// </summary>
    public const string FILE_LOCAL_FOLDER_FOR_WINDOWS = "FILE_LOCAL_FOLDER_FOR_WINDOWS";

    /// <summary>
    /// Unix系统本地目录
    /// </summary>
    public const string FILE_LOCAL_FOLDER_FOR_UNIX = "FILE_LOCAL_FOLDER_FOR_UNIX";

    /// <summary>
    /// MINIO文件AccessKey
    /// </summary>
    public const string FILE_MINIO_ACCESS_KEY = "FILE_MINIO_ACCESS_KEY";

    /// <summary>
    /// MINIO文件SecretKey
    /// </summary>
    public const string FILE_MINIO_SECRET_KEY = "FILE_MINIO_SECRET_KEY";

    /// <summary>
    /// MINIO文件EndPoint
    /// </summary>
    public const string FILE_MINIO_END_POINT = "FILE_MINIO_END_POINT";

    /// <summary>
    /// MINIO文件默认存储桶
    /// </summary>
    public const string FILE_MINIO_DEFAULT_BUCKET_NAME = "FILE_MINIO_DEFAULT_BUCKET_NAME";

    #endregion 存储引擎
}
