using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Teto.Plugin.Default.Services;
using Tml.Plugin.Extract.Services;

namespace Tml.Plugin.Extract.Modules;

public sealed class ExtractTmodModule : InteractionModuleBase<SocketInteractionContext>
{
    public required MessageSelectService MessageSelect { get; init; }

    public required TmlExtractService ModExtract { get; init; }

    [SlashCommand("extractmod", "Extracts .tmod files")]
    public async Task ExtractTmodAsync()
    {
        if (MessageSelect.GetUserMessage(Context.User, pop: false) is not { } msg)
        {
            await RespondAsync("No message selected!", ephemeral: true);
            return;
        }

        await ExtractMods(msg);
    }

    private async Task ExtractMods(IMessage message)
    {
        await RespondAsync("Extracting mods...");

        await ModExtract.ExtractAndUploadFilesAsync(
            message,
            async status =>
            {
                await ModifyOriginalResponseAsync(m =>
                    {
                        m.Embed = new EmbedBuilder()
                                 .WithTitle("Extracted mods")
                                 .WithCurrentTimestamp()
                                 .WithDescription(status)
                                 .Build();
                    }
                );
            }
        );
    }
}