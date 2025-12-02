using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Teto.Discord.Bot.Services;

internal sealed class BotStartService : IHostedService
{
    private readonly DiscordSocketClient client;
    private readonly IConfiguration config;

    public BotStartService(
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
        await client.LoginAsync(TokenType.Bot, config["TETO_BOT_TOKEN"]);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }
}
