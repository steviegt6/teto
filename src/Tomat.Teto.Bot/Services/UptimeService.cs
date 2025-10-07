using System;

namespace Tomat.Teto.Bot.Services;

public sealed class UptimeService
{
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;

    public TimeSpan Uptime => DateTimeOffset.UtcNow - StartTime;
}
