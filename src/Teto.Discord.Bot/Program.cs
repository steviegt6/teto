using System;
using System.Collections.Generic;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Teto.Discord;
using Teto.Discord.Bot;
using Teto.Plugin.Default;
using Tml.Plugin.Extract;
using Tml.Plugin.Id;
using Tml.Plugin.Tag;

var builder = Host.CreateApplicationBuilder(args);

var appSettingsPath = Environment.GetEnvironmentVariable("TETO_APP_SETTINGS_PATH") ?? "config/appsettings.json";
builder.Configuration.AddJsonFile(appSettingsPath);

// Transcribed and modified from the System Literate theme (NOT Console Literate
// theme!).
var consoleTheme = new SystemConsoleTheme(
    new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
    {
        [ConsoleThemeStyle.Text] = new() { Foreground = ConsoleColor.Gray },
        [ConsoleThemeStyle.SecondaryText] = new() { Foreground = ConsoleColor.Gray },
        [ConsoleThemeStyle.TertiaryText] = new() { Foreground = ConsoleColor.DarkGray },
        [ConsoleThemeStyle.Invalid] = new() { Foreground = ConsoleColor.Yellow },
        [ConsoleThemeStyle.Null] = new() { Foreground = ConsoleColor.Blue },
        [ConsoleThemeStyle.Name] = new() { Foreground = ConsoleColor.Gray },
        [ConsoleThemeStyle.String] = new() { Foreground = ConsoleColor.Cyan },
        [ConsoleThemeStyle.Number] = new() { Foreground = ConsoleColor.Magenta },
        [ConsoleThemeStyle.Boolean] = new() { Foreground = ConsoleColor.Blue },
        [ConsoleThemeStyle.Scalar] = new() { Foreground = ConsoleColor.Green },
        [ConsoleThemeStyle.LevelVerbose] = new() { Foreground = ConsoleColor.DarkGray },
        [ConsoleThemeStyle.LevelDebug] = new() { Foreground = ConsoleColor.DarkGray },
        [ConsoleThemeStyle.LevelInformation] = new() { Foreground = ConsoleColor.Gray },
        [ConsoleThemeStyle.LevelWarning] = new() { Foreground = ConsoleColor.Yellow },
        [ConsoleThemeStyle.LevelError] = new() { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
        [ConsoleThemeStyle.LevelFatal] = new() { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
    });

// TODO: Begin writing to file?
var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                 outputTemplate: "{Timestamp:HH:mm:ss} {Level:w3} {Message:lj}{NewLine}{Exception}",
                 theme: consoleTheme
             )
            .MinimumLevel.Information()
            .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
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
