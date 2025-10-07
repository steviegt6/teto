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

/*
builder.ConfigureAppConfiguration(
    config =>
    {
        config.AddJsonFile("config.json", optional: true);

        var tokenEnvVar = default(string?);
        bool foundToken;
        if (config.Properties.TryGetValue("token", out var tokenObj))
        {
            if (tokenObj is not string)
            {
                foundToken = false;
            }
            else if (tokenObj is string tokenString && tokenString.StartsWith('$'))
            {
                foundToken = false;
                tokenEnvVar = tokenString[1..];
            }
            else
            {
                foundToken = true;
            }
        }
        else
        {
            foundToken = false;
        }

        if (!foundToken)
        {
            tokenEnvVar ??= "TETO_BOT_TOKEN";
            var token = Environment.GetEnvironmentVariable(tokenEnvVar);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException($"Cannot start bot with invalid or unspecified token; please set the '{tokenEnvVar}' environment variable.");
            }

            config.Properties["token"] = token;
        }
    }
);
*/

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
