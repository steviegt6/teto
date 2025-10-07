using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Tomat.Teto.Plugin.Default.Services;

namespace Tomat.Teto.Plugin.Default.Modules;

public sealed class PasteModule : InteractionModuleBase<SocketInteractionContext>
{
    public enum Mode
    {
        Message,
        Attachments,
        Both,
    }
    
    public PasteService Paste { get; set; }

    public MessageSelectService MessageSelect { get; set; }

    [SlashCommand("genpastes", "Generates pastes")]
    public async Task GeneratePaste(Mode mode)
    {
        if (MessageSelect.GetUserMessage(Context.User, pop: false) is not { } msg)
        {
            await RespondAsync("No message selected!", ephemeral: true);
            return;
        }
        
        await GeneratePastes(msg, genMessage: mode is Mode.Message or Mode.Both, genAttachments: mode is Mode.Attachments or Mode.Both);
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