using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Teto.Core.Data;
using Teto.Core.Models.TmlTags;

const string db_path = "discord-tags.db";
var options = new DbContextOptionsBuilder<DiscordTagsDbContext>()
             .UseSqlite($"Data Source={db_path}")
             .Options;

using var db = new DiscordTagsDbContext(options);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

const string path = "tml_config.json";
var tmlConfig = File.ReadAllText(path);
var config = JsonSerializer.Deserialize<TmlConfig>(tmlConfig)!;

db.Tags.AddRange(
    config.GuildTags.Select(
        x => new Tag
        {
            OwnerSnowflake = x.OwnerId,
            TagName = x.Name,
            Message = x.Value,
            IsGlobal = x.IsGlobal,
        }
    )
);

db.SaveChanges();

sealed class GuildTag
{
    public ulong OwnerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool IsGlobal { get; set; }
}

sealed class TmlConfig
{
    public List<GuildTag> GuildTags { get; set; } = [];
}
