using Discord;
using Discord.Interactions;

using System;
using System.Threading.Tasks;

using Tomat.Teto.Bot.Services;

namespace Tomat.Teto.Bot.Modules.Terraria;

public sealed class IdLookupModule : InteractionModuleBase<SocketInteractionContext>
{
    public TmlIdService Ids { get; set; }

    [SlashCommand("ammo-id", description: "Gets data about an ammo using its ID or internal name.")]
    public async Task AmmoId(
        [Autocomplete(typeof(AmmoAutocomplete)), Summary("id", "Ammo ID or internal name.")] string id
    )
    {
        await ContentQuery("Ammo ID", "ammoid", id);
    }

    [SlashCommand("buff-id", description: "Gets data about a buff using its ID or internal name.")]
    public async Task BuffId(
        [Autocomplete(typeof(BuffAutocomplete)), Summary("id", "Buff ID or internal name.")] string id
    )
    {
        await ContentQuery("Buff ID", "buffid", id);
    }

    [SlashCommand("dust-id", description: "Gets data about a dust using its ID or internal name.")]
    public async Task DustId(
        [Autocomplete(typeof(DustAutocomplete)), Summary("id", "Dust ID or internal name.")] string id
    )
    {
        await ContentQuery("Dust ID", "dustid", id);
    }

    [SlashCommand("glowmask-id", description: "Gets data about a glow mask using its ID or internal name.")]
    public async Task GlowmaskId(
        [Autocomplete(typeof(GlowMaskAutocomplete)), Summary("id", "Glowmask ID or internal name.")] string id
    )
    {
        await ContentQuery("Glowmask ID", "glowmaskid", id);
    }

    [SlashCommand("gore-id", description: "Gets data about a gore using its ID or internal name.")]
    public async Task GoreId(
        [Autocomplete(typeof(GoreAutocomplete)), Summary("id", "Gore ID or internal name.")] string id
    )
    {
        await ContentQuery("Gore ID", "goreid", id);
    }

    [SlashCommand("item-id", description: "Gets data about an item using its ID or internal name.")]
    public async Task ItemId(
        [Autocomplete(typeof(ItemAutocomplete)), Summary("id", "Item ID or internal name.")] string id
    )
    {
        await ContentQuery("Item ID", "itemid", id);
    }

    [SlashCommand("mount-id", description: "Gets data about a mount using its ID or internal name.")]
    public async Task MountId(
        [Autocomplete(typeof(MountAutocomplete)), Summary("id", "Mount ID or internal name.")] string id
    )
    {
        await ContentQuery("Mount ID", "mountid", id);
    }

    [SlashCommand("npc-id", description: "Gets data about an NPC using its ID or internal name.")]
    public async Task NpcId(
        [Autocomplete(typeof(NPCAutocomplete)), Summary("id", "NPC ID or internal name.")] string id
    )
    {
        await ContentQuery("NPC ID", "npcid", id);
    }

    [SlashCommand("prefix-id", description: "Gets data about a prefix using its ID or internal name.")]
    public async Task PrefixId(
        [Autocomplete(typeof(PrefixAutocomplete)), Summary("id", "Prefix ID or internal name.")] string id
    )
    {
        await ContentQuery("Prefix ID", "prefixid", id);
    }

    [SlashCommand("projectile-id", description: "Gets data about a projectile using its ID or internal name.")]
    public async Task ProjectileId(
        [Autocomplete(typeof(ProjectileAutocomplete)), Summary("id", "Projectile ID or internal name.")] string id
    )
    {
        await ContentQuery("Projectile ID", "projectileid", id);
    }

    [SlashCommand("sound-id", description: "Gets data about a sound using its ID or internal name.")]
    public async Task SoundId(
        [Autocomplete(typeof(SoundAutocomplete)), Summary("id", "Sound ID or internal name.")] string id
    )
    {
        await ContentQuery("Sound ID", "soundid", id);
    }

    [SlashCommand("wall-id", description: "Gets data about a wall using its ID or internal name.")]
    public async Task WallId(
        [Autocomplete(typeof(WallAutocomplete)), Summary("id", "Wall ID or internal name.")] string id
    )
    {
        await ContentQuery("Wall ID", "wallid", id);
    }

    private async Task ContentQuery(string idDisplayName, string content, string id)
    {
        var search = Ids.SearchByContentType[content];

        if (!search.DataByNumericalId.TryGetValue(id, out var data)
         && !search.DataByInternalName.TryGetValue(id.ToLower(), out data))
        {
            await Failure();
            return;
        }

        if (data is null)
        {
            await Failure();
            return;
        }

        async Task Failure()
        {
            await RespondAsync(
                embed: new EmbedBuilder()
                      .WithTitle("ID not found")
                      .WithDescription($"Could not find content with the identifier \"{id}\".")
                      .WithCurrentTimestamp()
                      .Build()
            );
        }

        var builder = new EmbedBuilder()
                     .WithTitle($"{idDisplayName} data for '{id}'")
                     .WithCurrentTimestamp()
                     .AddField("# ID:", data.Id)
                     .AddField("Internal:", $"`{data.InternalName}`")
                     .AddField("Display Name:", data.DisplayName);

        if (data.Link != "No link")
        {
            builder.AddField("Wiki:", data.Link);
        }

        await RespondAsync(embed: builder.Build());
    }

    // todo: source gen these?
    private sealed class AmmoAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("ammoid", current)
                )
            );
        }
    }

    private sealed class BuffAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("buffid", current)
                )
            );
        }
    }

    private sealed class DustAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("dustid", current)
                )
            );
        }
    }

    private sealed class GlowMaskAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("glowmaskid", current)
                )
            );
        }
    }

    private sealed class GoreAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("goreid", current)
                )
            );
        }
    }

    private sealed class ItemAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("itemid", current)
                )
            );
        }
    }

    private sealed class MountAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("mountid", current)
                )
            );
        }
    }

    private sealed class NPCAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("npcid", current)
                )
            );
        }
    }

    private sealed class PrefixAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("prefixid", current)
                )
            );
        }
    }

    private sealed class ProjectileAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("projectileid", current)
                )
            );
        }
    }

    private sealed class SoundAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("soundid", current)
                )
            );
        }
    }

    private sealed class WallAutocomplete : AutocompleteHandler
    {
        public TmlIdService Ids { get; set; }

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            var current = autocompleteInteraction.Data.Current.Value?.ToString() ?? string.Empty;
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    Ids.GenerateContentAutos("wallid", current)
                )
            );
        }
    }
}