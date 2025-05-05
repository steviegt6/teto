using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Discord;

using Tomat.Teto.Bot.Models;

namespace Tomat.Teto.Bot.Services;

public sealed class TmlTagService
{
    private sealed class GuildTag
    {
        public ulong OwnerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public bool IsGlobal { get; set; }
    }

    private sealed class TmlConfig
    {
        public List<GuildTag> GuildTags { get; set; } = [];
    }

    private const string path = "tml_config.json";

    public Dictionary<string, TmlTag> GlobalTags { get; } = [];

    public Dictionary<TmlTagIdentity, TmlTag> Tags { get; } = [];

    private readonly AutocompleteResult[] global_autos;

    public TmlTagService()
    {
        var tmlConfig = File.ReadAllText(path);

        var config = JsonSerializer.Deserialize<TmlConfig>(tmlConfig);
        if (config is null)
        {
            throw new Exception();
        }

        foreach (var tag in config.GuildTags)
        {
            var model = new TmlTag(new TmlTagIdentity(tag.OwnerId, tag.Name), tag.Value, tag.IsGlobal);

            if (tag.IsGlobal)
            {
                GlobalTags[tag.Name.ToLowerInvariant()] = model;
            }

            Tags[model.Identity] = model;
        }

        global_autos = GlobalTags.Values.Select(x => new AutocompleteResult(x.Identity.Name, x.Identity.Name)).ToArray();
    }

    public IEnumerable<AutocompleteResult> GenerateGlobalAutos(string search)
    {
        var num = 0;

        foreach (var candidate in global_autos)
        {
            if (num >= 25)
            {
                yield break;
            }

            if (!candidate.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            num++;
            yield return candidate;
        }
    }
}