using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Discord;

using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot.Services;

public sealed class PasteService : IService
{
    private sealed class HasteResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;
    }

    private const string endpoint = "https://paste.ppeb.me";

    private readonly HttpClient http = new();

    public async Task<List<string>> GenerateLinks(IMessage message, bool genMessage, bool genAttachments)
    {
        var links = new List<string>();

        if (genMessage)
        {
            links.Add($"`message`: {await GenPaste(message.Content)}");
        }

        if (genAttachments)
        {
            foreach (var attachment in message.Attachments)
            {
                links.Add(
                    IsPermittedContentType(attachment.ContentType)
                        ? $"`{attachment.Filename}`: {await GenPaste(await Get(attachment.Url))}"
                        : $"`{attachment.Filename}`: unpermitted content type `{attachment.ContentType}`"
                );
            }
        }

        return links;
    }

    private async Task<string> Get(string url)
    {
        var response = await http.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> GenPaste(string contents)
    {
        var response = await http.PostAsync(endpoint + "/documents", new StringContent(contents));
        if (!response.IsSuccessStatusCode)
        {
            return $"error: {response.StatusCode}";
        }

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonSerializer.Deserialize<HasteResponse>(json);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Key))
        {
            return "bad response";
        }

        return endpoint + '/' + parsed.Key;
    }

    private static bool IsPermittedContentType(string contentType)
    {
        return contentType.StartsWith("text/", StringComparison.InvariantCultureIgnoreCase);
    }
}