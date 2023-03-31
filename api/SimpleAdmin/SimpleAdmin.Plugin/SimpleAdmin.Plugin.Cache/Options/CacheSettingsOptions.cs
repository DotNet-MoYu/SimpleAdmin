using Furion.ConfigurableOptions;

namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// 缓存设置
/// </summary>
public class CacheSettingsOptions : IConfigurableOptions
{

    /// <summary>
    /// 是否每次启动都清空
    /// </summary>
    public bool ClearRedis { get; set; } = false;


    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; }

}


