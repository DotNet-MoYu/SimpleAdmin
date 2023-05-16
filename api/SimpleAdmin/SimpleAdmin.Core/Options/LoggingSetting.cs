namespace SimpleAdmin.Core;

/// <summary>
/// 日志配置
/// </summary>
public class LoggingSetting
{
    /// <summary>
    /// LoggingMonitor配置
    /// </summary>
    public MonitorOptions Monitor { get; set; }

    /// <summary>
    /// 是否日志消息格式化
    /// </summary>
    public bool MessageFormat { get; set; }


    /// <summary>
    /// 日志等级
    /// </summary>
    public LogLevelOptions LogLevel { get; set; }

    /// <summary>
    /// LoggingMonitor配置
    /// </summary>
    public class MonitorOptions
    {
        /// <summary>
        /// 写入文件
        /// </summary>
        public bool Write { get; set; }

        /// <summary>
        /// 写入控制台
        /// </summary>
        public bool Console { get; set; }
    }


    /// <summary>
    /// 日志等级
    /// </summary>
    public class LogLevelOptions
    {
        /// <summary>
        /// 默认日志等级
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// 最大日志等级
        /// </summary>
        public string MaxLevel { get; set; } = "Error";
    }
}