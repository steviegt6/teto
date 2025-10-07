﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;

namespace Tomat.Teto.Plugin.Default.Modules;

public sealed class PluginModule : InteractionModuleBase<SocketInteractionContext>
{
    public IServiceProvider Services { get; set; }

    [SlashCommand("modules", "view loaded modules")]
    public async Task LoadedModules()
    {
        await RespondAsync(
            embed: new EmbedBuilder()
                  .WithTitle("Loaded modules")
                  .WithDescription(string.Join('\n', Services.GetServices<BotPlugin>().Select(x => $"- `{x.Description.UniqueName}` by \"{x.Description.Author}\"")))
                  .WithCurrentTimestamp()
                  .Build()
        );
    }
}
