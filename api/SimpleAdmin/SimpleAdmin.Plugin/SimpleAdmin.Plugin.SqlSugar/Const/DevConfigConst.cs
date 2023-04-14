namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 系统配置常量
/// </summary>
public class DevConfigConst
{
    /// <summary>
    /// 登录验证码开关
    /// </summary>
    public const string SYS_DEFAULT_CAPTCHA_OPEN = "SYS_DEFAULT_CAPTCHA_OPEN";

    /// <summary>
    /// 单用户登录开关
    /// </summary>
    public const string SYS_DEFAULT_SINGLE_OPEN = "SYS_DEFAULT_SINGLE_OPEN";

    /// <summary>
    /// 默认用户密码
    /// </summary>
    public const string SYS_DEFAULT_PASSWORD = "SYS_DEFAULT_PASSWORD";

    /// <summary>
    /// 系统默认工作台
    /// </summary>
    public const string SYS_DEFAULT_WORKBENCH_DATA = "SYS_DEFAULT_WORKBENCH_DATA";

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