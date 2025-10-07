using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Tomat.Teto;

public static class LoggerExtensions
{
    public static void LogDiscordMessage(this ILogger logger, LogMessage message)
    {
        Action<string?, object?[]>? logMethod = message.Severity switch
        {
            LogSeverity.Critical => logger.LogCritical,
            LogSeverity.Error => logger.LogError,
            LogSeverity.Warning => logger.LogWarning,
            LogSeverity.Info => logger.LogInformation,
            LogSeverity.Verbose => logger.LogDebug,
            LogSeverity.Debug => logger.LogTrace,
            _ => null,
        };

        logMethod?.Invoke(message.ToString(), []);
    }

    public static Func<LogMessage, Task> CreateDefaultLogHandler(this ILogger logger)
    {
        return message =>
        {
            logger.LogDiscordMessage(message);
            return Task.CompletedTask;
        };
    }
}
