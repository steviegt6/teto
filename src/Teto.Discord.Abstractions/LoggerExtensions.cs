using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Teto.Discord;

/// <summary>
///     <see cref="ILogger"/> extension methods for Discord bot interfaces.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    ///     The signature for a logging target designed to receive incoming
    ///     Discord messages.
    /// </summary>
    public delegate void DiscordMessageHandler(string? message, object?[] args);

    extension(ILogger logger)
    {
        /// <summary>
        ///     Gets the logger's message handler for the Discord log message.
        /// </summary>
        public DiscordMessageHandler? GetMessageHandler(LogMessage message)
        {
            ArgumentNullException.ThrowIfNull(logger);

            return message.Severity switch
            {
                LogSeverity.Critical => logger.LogCritical,
                LogSeverity.Error => logger.LogError,
                LogSeverity.Warning => logger.LogWarning,
                LogSeverity.Info => logger.LogInformation,
                LogSeverity.Verbose => logger.LogDebug,
                LogSeverity.Debug => logger.LogTrace,
                _ => null,
            };
        }

        /// <summary>
        ///     Logs a Discord message with proper severity formatting.
        /// </summary>
        public void LogDiscordMessage(LogMessage message)
        {
            ArgumentNullException.ThrowIfNull(logger);

            logger.GetMessageHandler(message)?.Invoke(message.ToString(), []);
        }

        /// <summary>
        ///     Produces a default implementation to subscribe to the various
        ///     Discord.NET logging events, derived from this logger.
        /// </summary>
        /// <returns></returns>
        public Func<LogMessage, Task> CreateDefaultLogHandler()
        {
            ArgumentNullException.ThrowIfNull(logger);

            return message =>
            {
                logger.LogDiscordMessage(message);
                return Task.CompletedTask;
            };
        }
    }
}
