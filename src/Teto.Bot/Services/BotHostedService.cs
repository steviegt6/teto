using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Teto.Bot.Services;

/// <summary>
///     Initializes the Discord bot client and handles starting and stopping it.
/// </summary>
public sealed class BotHostedService : IHostedService
{
    private readonly DiscordSocketClient client;
    private readonly IConfiguration config;

    public BotHostedService(
        DiscordSocketClient client,
        IConfiguration config,
        ILogger<DiscordSocketClient> logger
    )
    {
        this.client = client;
        this.config = config;

        client.Log += logger.CreateDefaultLogHandler();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await client.LoginAsync(TokenType.Bot, config.GetRequiredSection("app").GetRequiredSection("discord")["token"]);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }
}
