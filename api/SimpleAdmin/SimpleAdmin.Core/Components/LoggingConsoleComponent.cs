// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core;

/// <summary>
/// 日志写入文件的组件
/// </summary>
public sealed class LoggingConsoleComponent : IServiceComponent
{
    private readonly LoggingSetting _loggingSetting = App.GetConfig<LoggingSetting>("Logging", true);
    private readonly string _monitorName = "System.Logging.LoggingMonitor";

    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddConsoleFormatter(options =>
        {
            options.MessageFormat = logMsg =>
            {
                //如果不是LoggingMonitor日志或者开启了格式化才格式化
                if (logMsg.LogName != _monitorName && _loggingSetting.MessageFormat)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("【日志级别】：" + logMsg.LogLevel);
                    stringBuilder.AppendLine("【日志类名】：" + logMsg.LogName);
                    stringBuilder.AppendLine("【日志时间】：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    stringBuilder.AppendLine("【日志内容】：" + logMsg.Message);
                    if (logMsg.Exception != null)
                    {
                        stringBuilder.AppendLine("【异常信息】：" + logMsg.Exception);
                    }
                    return stringBuilder.ToString();
                }
                return logMsg.Message;
            };
            options.WriteHandler = (logMsg, scopeProvider, writer,
                fmtMsg, opt) =>
            {
                if (logMsg.LogName == _monitorName && !_loggingSetting.Monitor.Console) return;
                var consoleColor = ConsoleColor.White;
                switch (logMsg.LogLevel)
                {
                    case LogLevel.Information:
                        consoleColor = ConsoleColor.DarkGreen;
                        break;

                    case LogLevel.Warning:
                        consoleColor = ConsoleColor.DarkYellow;
                        break;

                    case LogLevel.Error:
                        consoleColor = ConsoleColor.DarkRed;
                        break;
                }
                writer.WriteWithColor(fmtMsg, ConsoleColor.Black, consoleColor);
            };
        });
    }
}

public static class TextWriterExtensions
{
    private const string DEFAULT_FOREGROUND_COLOR = "\x1B[39m\x1B[22m";
    private const string DEFAULT_BACKGROUND_COLOR = "\x1B[49m";

    public static void WriteWithColor(this TextWriter textWriter, string message, ConsoleColor? background,
        ConsoleColor? foreground)
    {
        // Order:
        //   1. background color
        //   2. foreground color
        //   3. message
        //   4. reset foreground color
        //   5. reset background color

        var backgroundColor = background.HasValue ? GetBackgroundColorEscapeCode(background.Value) : null;
        var foregroundColor = foreground.HasValue ? GetForegroundColorEscapeCode(foreground.Value) : null;

        if (backgroundColor != null)
        {
            textWriter.Write(backgroundColor);
        }
        if (foregroundColor != null)
        {
            textWriter.Write(foregroundColor);
        }

        textWriter.WriteLine(message);

        if (foregroundColor != null)
        {
            textWriter.Write(DEFAULT_FOREGROUND_COLOR);
        }
        if (backgroundColor != null)
        {
            textWriter.Write(DEFAULT_BACKGROUND_COLOR);
        }
    }

    private static string GetForegroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\x1B[30m",
            ConsoleColor.DarkRed => "\x1B[31m",
            ConsoleColor.DarkGreen => "\x1B[32m",
            ConsoleColor.DarkYellow => "\x1B[33m",
            ConsoleColor.DarkBlue => "\x1B[34m",
            ConsoleColor.DarkMagenta => "\x1B[35m",
            ConsoleColor.DarkCyan => "\x1B[36m",
            ConsoleColor.Gray => "\x1B[37m",
            ConsoleColor.Red => "\x1B[1m\x1B[31m",
            ConsoleColor.Green => "\x1B[1m\x1B[32m",
            ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
            ConsoleColor.Blue => "\x1B[1m\x1B[34m",
            ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
            ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
            ConsoleColor.White => "\x1B[1m\x1B[37m",

            _ => DEFAULT_FOREGROUND_COLOR
        };
    }

    private static string GetBackgroundColorEscapeCode(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => "\x1B[40m",
            ConsoleColor.DarkRed => "\x1B[41m",
            ConsoleColor.DarkGreen => "\x1B[42m",
            ConsoleColor.DarkYellow => "\x1B[43m",
            ConsoleColor.DarkBlue => "\x1B[44m",
            ConsoleColor.DarkMagenta => "\x1B[45m",
            ConsoleColor.DarkCyan => "\x1B[46m",
            ConsoleColor.Gray => "\x1B[47m",

            _ => DEFAULT_BACKGROUND_COLOR
        };
    }
}
