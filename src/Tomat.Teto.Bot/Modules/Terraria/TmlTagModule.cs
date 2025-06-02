using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Tomat.Teto.Bot.Models;
using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules.Terraria;

public sealed class TmlTagModule : InteractionModuleBase<SocketInteractionContext>
{
    private sealed class GenericTagAutocomplete : AutocompleteHandler
    {
        public TmlTagService Tags { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var user = autocompleteInteraction.Data.Options.FirstOrDefault(x => x.Name.Equals("user", StringComparison.InvariantCultureIgnoreCase))?.Value?.ToString() ?? string.Empty;

            return Task.FromResult(
                !string.IsNullOrWhiteSpace(user)
                    ? AutocompletionResult.FromSuccess(Tags.GenerateUserAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty, user))
                    : AutocompletionResult.FromSuccess(Tags.GenerateGlobalAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty))
            );
        }
    }

    private sealed class GlobalTagAutocomplete : AutocompleteHandler
    {
        public TmlTagService Tags { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            return Task.FromResult(AutocompletionResult.FromSuccess(Tags.GenerateGlobalAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty)));
        }
    }

    private sealed class AuthorTagAutocomplete : AutocompleteHandler
    {
        public TmlTagService Tags { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            return Task.FromResult(AutocompletionResult.FromSuccess(Tags.GenerateAuthorAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty)));
        }
    }

    private sealed class UserTagAutocomplete : AutocompleteHandler
    {
        public TmlTagService Tags { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            // return Task.FromResult(AutocompletionResult.FromSuccess(Tags.GenerateGlobalAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty)));
            var user = autocompleteInteraction.Data.Options.FirstOrDefault(x => x.Name.Equals("user", StringComparison.InvariantCultureIgnoreCase))?.Value?.ToString() ?? string.Empty;
            return Task.FromResult(AutocompletionResult.FromSuccess(Tags.GenerateUserAutos(autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty, user)));
        }
    }

    public DiscordSocketClient Client { get; set; }

    public TmlTagService Tags { get; set; }

    [SlashCommand("t", description: "Displays a global or user tML tag")]
    public async Task GenericTag(
        [Autocomplete(typeof(AuthorTagAutocomplete)), Discord.Interactions.Summary("user", "User @ or ID")] IUser? user = null,
        [Autocomplete(typeof(GenericTagAutocomplete)), Discord.Interactions.Summary("name", "Tag name")] string? name = null
    )
    {
        if (name is null)
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("Please input tag name")
                      .WithDescription("Cannot search with just a user or no inputs.")
                      .WithCurrentTimestamp()
                      .Build()
            );
            return;
        }

        if (user is not null)
        {
            if (!Tags.UserTags.TryGetValue(user.Id.ToString(), out var userTags))
            {
                await RespondAsync(
                    embed: new EmbedBuilder()
                          .WithTitle("User not found")
                          .WithDescription("The user has no known tags")
                          .WithCurrentTimestamp()
                          .Build()
                );
                return;
            }

            await UserTag(user, name);
        }
        else
        {
            if (!Tags.GlobalTags.TryGetValue(name.ToLowerInvariant(), out var tag))
            {
                await RespondAsync(
                    embed: new EmbedBuilder()
                          .WithTitle("Tag not found")
                          .WithDescription($"Could not find a global tag with the name \"{name}\"")
                          .WithCurrentTimestamp()
                          .Build()
                );
                return;
            }

            await GlobalTag(name);
        }
    }

    [SlashCommand("global-tag", description: "Displays a global tML tag")]
    public async Task GlobalTag(
        [Autocomplete(typeof(GlobalTagAutocomplete)), Discord.Interactions.Summary("name", "Tag name")] string name
    )
    {
        if (!Tags.GlobalTags.TryGetValue(name.ToLowerInvariant(), out var tag))
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("Tag not found")
                      .WithDescription($"Could not find a global tag with the name \"{name}\"")
                      .WithCurrentTimestamp()
                      .Build()
            );
            return;
        }

        await DisplayTag(tag);
    }

    [SlashCommand("user-tag", description: "Displays a user-specific tML tag")]
    public async Task UserTag(
        [Autocomplete(typeof(AuthorTagAutocomplete)), Discord.Interactions.Summary("user", "User @ or ID")] IUser user,
        [Autocomplete(typeof(UserTagAutocomplete)), Discord.Interactions.Summary("name", "Tag name")] string name
    )
    {
        if (!Tags.UserTags.TryGetValue(user.Id.ToString(), out var userTags))
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("User not found")
                      .WithDescription("The user has no known tags")
                      .WithCurrentTimestamp()
                      .Build()
            );
            return;
        }

        if (!userTags.TryGetValue(name, out var tag))
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("Tag not found")
                      .WithDescription($"Could not find a user tag with the name \"{name}\" belonging to the given user")
                      .WithCurrentTimestamp()
                      .Build()
            );
            return;
        }

        await DisplayTag(tag);
    }

    private async Task DisplayTag(TmlTag tag)
    {
        var message = tag.Value + $"\n-# tag: {tag.Identity.Name} (owner: {tag.Identity.OwnerString}, global: {tag.IsGlobal.ToString().ToLowerInvariant()})";

        await RespondAsync(message);
    }
}