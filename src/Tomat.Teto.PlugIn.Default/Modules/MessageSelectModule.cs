using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Tomat.Teto.Plugin.Default.Services;

namespace Tomat.Teto.Plugin.Default.Modules;

public sealed class MessageSelectModule : InteractionModuleBase<SocketInteractionContext>
{
    public MessageSelectService MessageSelect { get; set; }

    [MessageCommand("Select message (for commands)")]
    public async Task SelectMessageAsync(IMessage message)
    {
        MessageSelect.SetUserMessage(Context.User, message);

        await RespondAsync(text: "Message selected!", ephemeral: true);
    }
}