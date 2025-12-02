using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Tml.Plugin.Id.Services;

namespace Tml.Plugin.Id.Modules;

public sealed class TmlIdModule : InteractionModuleBase<SocketInteractionContext>
{
    public required TmlIdService IdLookup { get; init; }

    [SlashCommand("ammoid", description: "Gets data about an ammo using its ID or internal name.")]
    public async Task AmmoId(
        [Autocomplete<AmmoAutocomplete>, Summary("id", "Ammo ID or internal name.")] string id
    )
    {
        await ContentQuery("Ammo ID", "ammoid", id);
    }

    [SlashCommand("buffid", description: "Gets data about a buff using its ID or internal name.")]
    public async Task BuffId(
        [Autocomplete<BuffAutocomplete>, Summary("id", "Buff ID or internal name.")] string id
    )
    {
        await ContentQuery("Buff ID", "buffid", id);
    }

    [SlashCommand("dustid", description: "Gets data about a dust using its ID or internal name.")]
    public async Task DustId(
        [Autocomplete<DustAutocomplete>, Summary("id", "Dust ID or internal name.")] string id
    )
    {
        await ContentQuery("Dust ID", "dustid", id);
    }

    [SlashCommand("glowmaskid", description: "Gets data about a glow mask using its ID or internal name.")]
    public async Task GlowmaskId(
        [Autocomplete<GlowMaskAutocomplete>, Summary("id", "Glowmask ID or internal name.")] string id
    )
    {
        await ContentQuery("Glowmask ID", "glowmaskid", id);
    }

    [SlashCommand("goreid", description: "Gets data about a gore using its ID or internal name.")]
    public async Task GoreId(
        [Autocomplete<GoreAutocomplete>, Summary("id", "Gore ID or internal name.")] string id
    )
    {
        await ContentQuery("Gore ID", "goreid", id);
    }

    [SlashCommand("itemid", description: "Gets data about an item using its ID or internal name.")]
    public async Task ItemId(
        [Autocomplete<ItemAutocomplete>, Summary("id", "Item ID or internal name.")] string id
    )
    {
        await ContentQuery("Item ID", "itemid", id);
    }

    [SlashCommand("mountid", description: "Gets data about a mount using its ID or internal name.")]
    public async Task MountId(
        [Autocomplete<MountAutocomplete>, Summary("id", "Mount ID or internal name.")] string id
    )
    {
        await ContentQuery("Mount ID", "mountid", id);
    }

    [SlashCommand("npcid", description: "Gets data about an NPC using its ID or internal name.")]
    public async Task NpcId(
        [Autocomplete<NpcAutocomplete>, Summary("id", "NPC ID or internal name.")] string id
    )
    {
        await ContentQuery("NPC ID", "npcid", id);
    }

    [SlashCommand("prefixid", description: "Gets data about a prefix using its ID or internal name.")]
    public async Task PrefixId(
        [Autocomplete<PrefixAutocomplete>, Summary("id", "Prefix ID or internal name.")] string id
    )
    {
        await ContentQuery("Prefix ID", "prefixid", id);
    }

    [SlashCommand("projectileid", description: "Gets data about a projectile using its ID or internal name.")]
    public async Task ProjectileId(
        [Autocomplete<ProjectileAutocomplete>, Summary("id", "Projectile ID or internal name.")] string id
    )
    {
        await ContentQuery("Projectile ID", "projectileid", id);
    }

    [SlashCommand("soundid", description: "Gets data about a sound using its ID or internal name.")]
    public async Task SoundId(
        [Autocomplete<SoundAutocomplete>, Summary("id", "Sound ID or internal name.")] string id
    )
    {
        await ContentQuery("Sound ID", "soundid", id);
    }

    [SlashCommand("wallid", description: "Gets data about a wall using its ID or internal name.")]
    public async Task WallId(
        [Autocomplete<WallAutocomplete>, Summary("id", "Wall ID or internal name.")] string id
    )
    {
        await ContentQuery("Wall ID", "wallid", id);
    }

    private async Task ContentQuery(string idDisplayName, string content, string id)
    {
        var search = IdLookup.SearchByContentType[content];

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

    private abstract class AbstractIdAutocomplete(string id) : AutocompleteHandler
    {
        public required TmlIdService Ids { get; init; }

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
                    Ids.GenerateContentAutos(id, current)
                )
            );
        }
    }

    private sealed class AmmoAutocomplete() : AbstractIdAutocomplete("ammoid");

    private sealed class BuffAutocomplete() : AbstractIdAutocomplete("buffid");

    private sealed class DustAutocomplete() : AbstractIdAutocomplete("dustid");

    private sealed class GlowMaskAutocomplete() : AbstractIdAutocomplete("glowmaskid");

    private sealed class GoreAutocomplete() : AbstractIdAutocomplete("goreid");

    private sealed class ItemAutocomplete() : AbstractIdAutocomplete("itemid");

    private sealed class MountAutocomplete() : AbstractIdAutocomplete("mountid");

    private sealed class NpcAutocomplete() : AbstractIdAutocomplete("npcid");

    private sealed class PrefixAutocomplete() : AbstractIdAutocomplete("prefixid");

    private sealed class ProjectileAutocomplete() : AbstractIdAutocomplete("projectileid");

    private sealed class SoundAutocomplete() : AbstractIdAutocomplete("soundid");

    private sealed class WallAutocomplete() : AbstractIdAutocomplete("wallid");
}
