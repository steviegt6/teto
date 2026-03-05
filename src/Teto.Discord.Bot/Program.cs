using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Teto.Discord;
using Teto.Discord.Bot;
using Teto.Plugin.Default;
using Tml.Plugin.Extract;
using Tml.Plugin.Id;
using Tml.Plugin.Tag;

var builder = Host.CreateApplicationBuilder(args);

// TODO: Begin writing to file?
var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddSerilog(logger);

// TODO: Look into proper configuration values.
builder.Services.AddSingleton(
    new DiscordSocketConfig
    {
        AlwaysDownloadUsers = true,
    }
);

// Singletons provided by Discord.NET needed elsewhere.
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(
    sp =>
    {
        var client = sp.GetRequiredService<DiscordSocketClient>();
        return new InteractionService(client);
    }
);

builder.Services.AddBotPlugin<DefaultPlugin>();

builder.Services.AddBotPlugin<TmlExtractPlugin>();
builder.Services.AddBotPlugin<TmlIdPlugin>();
builder.Services.AddBotPlugin<TmlTagPlugin>();

builder.Services.AddBotPlugin<HostPlugin>();

using var host = builder.Build();

await host.RunAsync();
