using System;

namespace Teto.Plugin.Default.Services;

public sealed class UptimeService
{
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;

    public TimeSpan Uptime => DateTimeOffset.UtcNow - StartTime;
}
