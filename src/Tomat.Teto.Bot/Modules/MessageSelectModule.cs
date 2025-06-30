using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules;

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