using System.Collections.Generic;

using Discord;

using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot.Services;

public sealed class MessageSelectService : IService
{
    private readonly Dictionary<ulong, IMessage?> selectedMessages = [];

    public IMessage? GetUserMessage(IUser user, bool pop)
    {
        if (!selectedMessages.TryGetValue(user.Id, out var msg))
        {
            return null;
        }

        if (pop)
        {
            selectedMessages[user.Id] = null;
        }

        return msg;
    }

    public void SetUserMessage(IUser user, IMessage message)
    {
        selectedMessages[user.Id] = message;
    }
}