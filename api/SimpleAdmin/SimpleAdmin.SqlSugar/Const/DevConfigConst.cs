namespace SimpleAdmin.SqlSugar;

/// <summary>
/// 系统配置常量
/// </summary>
public class DevConfigConst
{
    /// <summary>
    /// 系统默认工作台
    /// </summary>
    public const string SYS_DEFAULT_WORKBENCH_DATA = "SYS_DEFAULT_WORKBENCH_DATA";

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
    public const string LOGIN_SINGLE_OPEN = "LOGIN_DEFAULT_SINGLE_OPEN";

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

    #endregion

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

    #endregion

    #region MQTT

    /// <summary>
    /// mqtt连接地址
    /// </summary>
    public const string MQTT_PARAM_URL = "MQTT_PARAM_URL";

    /// <summary>
    /// mqtt连接用户名
    /// </summary>
    public const string MQTT_PARAM_USERNAME = "MQTT_PARAM_USERNAME";

    /// <summary>
    /// mqtt连接密码
    /// </summary>
    public const string MQTT_PARAM_PASSWORD = "MQTT_PARAM_PASSWORD";

    #endregion MQTT

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
    /// MINIO文件SecetKey
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