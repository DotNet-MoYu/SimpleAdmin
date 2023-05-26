namespace SimpleAdmin.SqlSugar;

/// <summary>
/// 字典常量
/// </summary>
public class DevDictConst
{
    #region 系统字典

    /// <summary>
    /// 性别
    /// </summary>
    public const string GENDER = "GENDER";

    /// <summary>
    /// 名族
    /// </summary>
    public const string NATION = "NATION";

    /// <summary>
    /// 用户证件类型
    /// </summary>
    public const string IDCARD_TYPE = "IDCARD_TYPE";

    /// <summary>
    /// 通用文化程度
    /// </summary>
    public const string CULTURE_LEVEL = "CULTURE_LEVEL";



    #region 系统通用状态

    /// <summary>
    /// 启用
    /// </summary>
    public const string COMMON_STATUS_ENABLE = "ENABLE";

    /// <summary>
    /// 停用
    /// </summary>
    public const string COMMON_STATUS_DISABLED = "DISABLED";

    #endregion 系统通用状态

    #region 在线用户状态

    /// <summary>
    /// 在线
    /// </summary>
    public const string ONLINE_STATUS_ONLINE = "ONLINE";

    /// <summary>
    /// 离线
    /// </summary>
    public const string ONLINE_STATUS_OFFLINE = "OFFLINE";

    #endregion 在线用户状态

    #region 上传文件引擎

    /// <summary>
    /// 本地
    /// </summary>
    public const string FILE_ENGINE_LOCAL = "LOCAL";

    /// <summary>
    /// MINIO
    /// </summary>
    public const string FILE_ENGINE_MINIO = "MINIO";

    #endregion 上传文件引擎

    #endregion 系统字典
}