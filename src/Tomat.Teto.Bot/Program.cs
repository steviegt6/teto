using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tomat.Teto.Bot.Services;
using Tomat.Teto.Bot.Services.Hosting;
using Tomat.Teto.Bot.Services.Tml;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging(
    loggingBuilder =>
    {
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
    }
);

builder.ConfigureServices(
    services =>
    {
        // TODO: Look into proper configuration values.
        services.AddSingleton(
            new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
            }
        );

        // Singletons provided by Discord.NET needed elsewhere.
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(
            sp =>
            {
                var client = sp.GetRequiredService<DiscordSocketClient>();
                return new InteractionService(client);
            }
        );

        // Hosted services used to initialize the mod.
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<BotStartService>();

        services.AddSingleton<MessageSelectService>();
        services.AddSingleton<PasteService>();
        services.AddSingleton<UptimeService>();

        services.AddSingleton<ModExtractService>();
        services.AddSingleton<TmlTagService>();
        services.AddSingleton<TmlIdService>();
    }
);

using var host = builder.Build();

await host.RunAsync();
