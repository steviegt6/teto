using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Discord;

using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot.Services;

public sealed class TmlIdService : IService
{
    public sealed class IdData(string id, string displayName, string link, string internalName)
    {
        public string Id { get; set; } = id;

        public string DisplayName { get; set; } = displayName;

        public string Link { get; set; } = link;

        public string InternalName { get; set; } = internalName;
    }

    public sealed class IdSearch
    {
        public Dictionary<string, IdData?> DataByNumericalId { get; set; } = [];

        public Dictionary<string, IdData?> DataByInternalName { get; set; } = [];
    }

    private sealed class IdCollection
    {
        public List<IdData> Ids { get; set; } = [];
    }

    private const int auto_max = 25;
    private const string id_directory = "IdData";

    private readonly Dictionary<string, IdSearch> searchByContentType;

    private readonly Dictionary<string, List<AutocompleteResult>> autocompleteByContentType;

    public IReadOnlyDictionary<string, IdSearch> SearchByContentType => searchByContentType;

    public TmlIdService()
    {
        searchByContentType = [];
        autocompleteByContentType = [];

        var idFiles = Directory.GetFiles(id_directory);

        foreach (var idFile in idFiles)
        {
            var data = File.ReadAllText(idFile);

            var idData = JsonSerializer.Deserialize<IdCollection>(data) ?? throw new Exception();

            var lowerId = Path.GetFileNameWithoutExtension(idFile).ToLower();

            // Remove the trailing s from 'ids'.
            var idName = lowerId[..^1];

            IdSearch search = new();

            autocompleteByContentType[idName] = [];

            foreach (var entry in idData.Ids)
            {
                var correctedDualId = entry.Id.Replace(" ", "_");

                // Replace HTML spaces with a regular one.
                entry.DisplayName = entry.DisplayName.Replace("&nbsp;", " ");

                search.DataByNumericalId[correctedDualId] = entry;

                // If an entry doesn't have an internal name at all, skip its addition since there's unlikely to be any useful info here.
                if (entry.InternalName == "n/a" || string.IsNullOrEmpty(entry.InternalName))
                {
                    continue;
                }

                search.DataByInternalName[entry.InternalName.ToLower()] = entry;

                autocompleteByContentType[idName].Add(new AutocompleteResult(entry.InternalName, entry.InternalName));
                autocompleteByContentType[idName].Add(new AutocompleteResult(entry.Id, entry.Id));
            }

            searchByContentType[idName] = search;
        }
    }

    public IEnumerable<AutocompleteResult> GenerateContentAutos(string content, string text)
    {
        var num = 0;

        foreach (var candidate in autocompleteByContentType[content])
        {
            if (num >= auto_max)
            {
                yield break;
            }

            if (!candidate.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            num++;
            yield return candidate;
        }
    }
}