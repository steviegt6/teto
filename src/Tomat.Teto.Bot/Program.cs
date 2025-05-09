using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.DependencyInjection;
using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot;

internal static class Program
{
    public static async Task Main()
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
            services.TryAddService(new InteractionService(services.ExpectService<DiscordSocketClient>()));
            services.TryAddService<InteractionHandler>();
            services.TryAddService<TmlTagService>();
            services.TryAddService<TmlIdService>();
            services.TryAddService<PasteService>();
        }

        var client = services.ExpectService<DiscordSocketClient>();

        client.Log += async msg =>
        {
            Console.WriteLine(msg);
            await Task.CompletedTask;
        };

        services.TryAddService(new InteractionService(client.Rest));

        await services.ExpectService<InteractionHandler>().InitializeAsync();

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}