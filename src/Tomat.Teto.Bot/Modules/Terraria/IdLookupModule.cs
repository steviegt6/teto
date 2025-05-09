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
    public async Task AmmoId([Autocomplete(typeof(AmmoAutocomplete)), Summary("AmmoID", "Ammo ID or internal name.")] string id)
        => await ContentQuery("ammoid", id);

    [SlashCommand("buff-id", description: "Gets data about a buff using its ID or internal name.")]
    public async Task BuffId([Autocomplete(typeof(BuffAutocomplete)), Summary("BuffID", "Buff ID or internal name.")] string id)
        => await ContentQuery("buffid", id);

    [SlashCommand("dust-id", description: "Gets data about a dust using its ID or internal name.")]
    public async Task DustId([Autocomplete(typeof(DustAutocomplete)), Summary("DustID", "Dust ID or internal name.")] string id)
        => await ContentQuery("dustid", id);

    [SlashCommand("glowmask-id", description: "Gets data about a glow mask using its ID or internal name.")]
    public async Task GlowMaskId([Autocomplete(typeof(GlowMaskAutocomplete)), Summary("GlowMaskID", "GlowMask ID or internal name.")] string id)
        => await ContentQuery("glowmaskid", id);

    [SlashCommand("gore-id", description: "Gets data about a gore using its ID or internal name.")]
    public async Task GoreId([Autocomplete(typeof(GoreAutocomplete)), Summary("GoreID", "Gore ID or internal name.")] string id)
        => await ContentQuery("goreid", id);

    [SlashCommand("item-id", description: "Gets data about an item using its ID or internal name.")]
    public async Task ItemId([Autocomplete(typeof(ItemAutocomplete)), Summary("ItemID", "Item ID or internal name.")] string id)
        => await ContentQuery("itemid", id);

    [SlashCommand("mount-id", description: "Gets data about a mount using its ID or internal name.")]
    public async Task MountId([Autocomplete(typeof(MountAutocomplete)), Summary("MountID", "Mount ID or internal name.")] string id)
        => await ContentQuery("mountid", id);

    [SlashCommand("npc-id", description: "Gets data about an NPC using its ID or internal name.")]
    public async Task NPCId([Autocomplete(typeof(NPCAutocomplete)), Summary("NPCID", "NPC ID or internal name.")] string id)
        => await ContentQuery("npcid", id);

    [SlashCommand("prefix-id", description: "Gets data about a prefix using its ID or internal name.")]
    public async Task PrefixId([Autocomplete(typeof(PrefixAutocomplete)), Summary("PrefixID", "Prefix ID or internal name.")] string id)
        => await ContentQuery("prefixid", id);

    [SlashCommand("projectile-id", description: "Gets data about a projectile using its ID or internal name.")]
    public async Task ProjectileId([Autocomplete(typeof(ProjectileAutocomplete)), Summary("ProjectileID", "Projectile ID or internal name.")] string id)
        => await ContentQuery("projectileid", id);

    [SlashCommand("sound-id", description: "Gets data about a sound using its ID or internal name.")]
    public async Task SoundId([Autocomplete(typeof(SoundAutocomplete)), Summary("SoundID", "Sound ID or internal name.")] string id)
        => await ContentQuery("soundid", id);

    [SlashCommand("wall-id", description: "Gets data about a wall using its ID or internal name.")]
    public async Task WallId([Autocomplete(typeof(WallAutocomplete)), Summary("WallID", "Wall ID or internal name.")] string id)
        => await ContentQuery("wallid", id);

    public async Task ContentQuery(string content, string id)
    {
        TmlIdService.IdSearch search = Ids.SearchByContentType[content];

        TmlIdService.IdData data = null;

        if (!search.DataByNumericalId.TryGetValue(id, out data))
        {
            if (!search.DataByInternalName.TryGetValue(id.ToLower(), out data))
            {
                await Failure();
                return;
            }
        }

        if (data == null)
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
                .Build());
        }

        var builder = new EmbedBuilder()
            .WithTitle($"{content.ToUpper()} data for '{id}'")
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("ammoid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("buffid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("dustid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("glowmaskid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("goreid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("itemid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("mountid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("npcid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("prefixid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("projectileid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("soundid", current)
            ));
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
            return Task.FromResult(AutocompletionResult.FromSuccess(
                Ids.GenerateContentAutos("wallid", current)
            ));
        }
    }
}