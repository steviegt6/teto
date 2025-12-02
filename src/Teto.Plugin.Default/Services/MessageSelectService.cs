using System.Collections.Generic;
using Discord;

namespace Teto.Plugin.Default.Services;

public sealed class MessageSelectService
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
