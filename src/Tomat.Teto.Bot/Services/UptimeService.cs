using System;

using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot.Services;

public sealed class UptimeService : IService
{
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;

    public TimeSpan Uptime => DateTimeOffset.UtcNow - StartTime;
}