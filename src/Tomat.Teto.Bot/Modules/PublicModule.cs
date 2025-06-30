using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules;

public sealed class PublicModule : InteractionModuleBase<SocketInteractionContext>
{
    public DiscordSocketClient Client { get; set; }

    public InteractionService Commands { get; set; }

    public InteractionHandler Handler { get; set; }

    public UptimeService UptimeService { get; set; }

    [SlashCommand("status", "bot latency and other info")]
    public async Task StatusAsync()
    {
        var s = Stopwatch.StartNew();
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("Pong!")
                      .WithDescription($"Latency: {Client.Latency}ms")
                      .WithCurrentTimestamp()
                      .Build()
            );
            s.Stop();
        }
        var responseLatency = s.ElapsedMilliseconds;

        await ModifyOriginalResponseAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                         .WithTitle("Pong!")
                         .WithDescription($"Latency: {Client.Latency}ms"
                                        + $"\nMessage delta: {responseLatency}ms"
                                        + $"\n"
                                        + $"\nBot up-time: <t:{UptimeService.StartTime.ToUnixTimeSeconds()}:R> ({UptimeService.Uptime.Days}d {UptimeService.Uptime.Hours}h {UptimeService.Uptime.Minutes}m {UptimeService.Uptime.Seconds}s)")
                         .WithCurrentTimestamp()
                         .Build();
            }
        );
    }
}