using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var token = Environment.GetEnvironmentVariable("TETO_BOT_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Cannot start bot with invalid or unspecified token; please set the TETO_BOT_TOKEN environment variable.");
        }

        var services = new ServiceProvider();
        {
            services.TryAddService(new DiscordSocketConfig()); // todo
            services.TryAddService<DiscordSocketClient>();
        }

        var client = services.ExpectService<DiscordSocketClient>();
        {
            client.Log += async msg =>
            {
                Console.WriteLine(msg);
                await Task.CompletedTask;
            };
        }

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}