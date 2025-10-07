using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tomat.Teto;
using Tomat.Teto.Bot;
using Tomat.Teto.Plugin.Default;
using Tomat.Teto.Plugin.Tml.Extract;
using Tomat.Teto.Plugin.Tml.Id;
using Tomat.Teto.Plugin.Tml.Tag;

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

        services.AddBotPlugin<DefaultPlugin>();

        services.AddBotPlugin<TmlExtractPlugin>();
        services.AddBotPlugin<TmlIdPlugin>();
        services.AddBotPlugin<TmlTagPlugin>();

        services.AddBotPlugin<HostPlugin>();
    }
);

using var host = builder.Build();

await host.RunAsync();
