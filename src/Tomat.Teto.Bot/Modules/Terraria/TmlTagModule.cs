using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules.Terraria;

public sealed class TmlTagModule : InteractionModuleBase<SocketInteractionContext>
{
    private sealed class GlobalTagAutocomplete : AutocompleteHandler
    {
        public TmlTagService Tags { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            return Task.FromResult(AutocompletionResult.FromSuccess(Tags.GenerateGlobalAutos(autocompleteInteraction.Data.Options.First()?.Value?.ToString() ?? string.Empty)));
        }

        private static IEnumerable<T> Truncate<T>(IEnumerable<T> values, int limit)
        {
            var num = 0;
            foreach (var value in values)
            {
                if (num >= limit)
                {
                    break;
                }

                num++;
                yield return value;
            }
        }
    }

    public DiscordSocketClient Client { get; set; }

    public TmlTagService Tags { get; set; }

    [SlashCommand("global-tag", description: "Displays a global tML tag")]
    public async Task GlobalTag([Autocomplete(typeof(GlobalTagAutocomplete))] string name)
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
        if (user is not null)
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle(tag.Identity.Name)
                      .WithDescription(tag.Value)
                      .WithAuthor(name: user.GlobalName, iconUrl: user.GetAvatarUrl())
                      .Build()
            );
        }
        else
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle(tag.Identity.Name)
                      .WithDescription(tag.Value)
                      .Build()
            );
        }
    }

    public async Task UserTag(string name, IUser user) { }
}