using System.Threading.Tasks;

using Discord;
using Discord.Interactions;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules;

public sealed class PasteModule : InteractionModuleBase<SocketInteractionContext>
{
    public PasteService Paste { get; set; }

    [MessageCommand("Hastebin: messages & attachments")]
    public async Task GeneratePasteAll(IMessage message)
    {
        await GeneratePastes(message, genMessage: true, genAttachments: true);
    }

    [MessageCommand("Hastebin: message")]
    public async Task GeneratePasteMessage(IMessage message)
    {
        await GeneratePastes(message, genMessage: true, genAttachments: false);
    }

    [MessageCommand("Hastebin: attachments")]
    public async Task GeneratePasteAttachments(IMessage message)
    {
        await GeneratePastes(message, genMessage: false, genAttachments: true);
    }

    private async Task GeneratePastes(IMessage message, bool genMessage, bool genAttachments)
    {
        await RespondAsync("Generating pastes...");

        if (genAttachments && !genMessage && message.Attachments.Count == 0)
        {
            await ModifyOriginalResponseAsync(
                x =>
                {
                    x.Content = null;
                    x.Embed = new EmbedBuilder()
                             .WithTitle("No attachments")
                             .WithDescription("Cannot make pastes for message with no attachments")
                             .WithCurrentTimestamp()
                             .Build();
                }
            );
            return;
        }

        var links = await Paste.GenerateLinks(message, genMessage, genAttachments);
        await ModifyOriginalResponseAsync(
            x =>
            {
                x.Content = null;
                x.Embed = new EmbedBuilder()
                         .WithTitle("Pastes")
                         .WithDescription(string.Join('\n', links))
                         .WithCurrentTimestamp()
                         .Build();
            }
        );
    }
}