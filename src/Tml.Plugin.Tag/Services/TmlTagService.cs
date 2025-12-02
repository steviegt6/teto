using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Discord;
using Tml.Plugin.Tag.Models;

namespace Tml.Plugin.Tag.Services;

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

    private const int auto_max = 25;
    private const string path = "tml_config.json";

    public Dictionary<string, TmlTag> GlobalTags { get; } = [];

    public Dictionary<TmlTagIdentity, TmlTag> Tags { get; } = [];

    public Dictionary<string, Dictionary<string, TmlTag>> UserTags { get; } = [];

    private readonly AutocompleteResult[] globalAutos;
    private readonly AutocompleteResult[] usersWithCommands;

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

        foreach (var tag in Tags.Values)
        {
            if (!UserTags.TryGetValue(tag.Identity.OwnerString, out var userTags))
            {
                userTags = UserTags[tag.Identity.OwnerString] = [];
            }

            userTags[tag.Identity.Name] = tag;
        }

        globalAutos = GlobalTags.Values.Select(x => new AutocompleteResult(x.Identity.Name, x.Identity.Name)).ToArray();
        usersWithCommands = Tags.Select(x => new AutocompleteResult(x.Key.OwnerString, x.Key.OwnerString)).Distinct().ToArray();
    }

    public IEnumerable<AutocompleteResult> GenerateGlobalAutos(string search)
    {
        var num = 0;

        foreach (var candidate in globalAutos)
        {
            if (num >= auto_max)
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

    public IEnumerable<AutocompleteResult> GenerateAuthorAutos(string search)
    {
        var num = 0;

        foreach (var candidate in usersWithCommands)
        {
            if (num >= auto_max)
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

    public IEnumerable<AutocompleteResult> GenerateUserAutos(string search, string userId)
    {
        if (!UserTags.TryGetValue(userId, out var userTags))
        {
            yield break;
        }

        var num = 0;

        foreach (var candidate in userTags)
        {
            if (num >= auto_max)
            {
                yield break;
            }

            if (!candidate.Value.Identity.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            num++;
            yield return new AutocompleteResult(candidate.Value.Identity.Name, candidate.Value.Identity.Name);
        }
    }
}