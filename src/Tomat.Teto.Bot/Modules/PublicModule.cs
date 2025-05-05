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

    [SlashCommand("ping", "latency")]
    public async Task PingAsync()
    {
        var embed = new EmbedBuilder()
                   .WithTitle("Pong!")
                   .WithDescription($"Latency: {Client.Latency}ms")
                   .WithCurrentTimestamp()
                   .Build();

        await RespondAsync(embed: embed);
    }
}