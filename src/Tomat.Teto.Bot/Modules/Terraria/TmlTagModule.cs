using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules.Terraria;

public sealed class TmlTagModule : InteractionModuleBase<SocketInteractionContext>
{
    public DiscordSocketClient Client { get; set; }

    public TmlTagService Tags { get; set; }

    [SlashCommand("global-tag", description: "Displays a global tML tag")]
    public async Task GlobalTag(string name)
    {
        if (!Tags.GlobalTags.TryGetValue(name.ToLowerInvariant(), out var tag))
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("Tag not found")
                      .WithDescription($"Could not find a glboal tag with the name \"{name}\"")
                      .WithCurrentTimestamp()
                      .Build()
            );
            return;
        }

        var user = Client.GetUser(tag.Identity.Owner);
        var userName = user?.GlobalName ?? "unknown";
        var iconUrl = user?.GetAvatarUrl();

        await RespondAsync(
            embed: new EmbedBuilder()
                  .WithTitle(tag.Identity.Name)
                  .WithDescription(tag.Value)
                  .WithAuthor(name: userName, iconUrl: iconUrl)
                  .Build()
        );
    }

    public async Task UserTag(string name, IUser user) { }
}