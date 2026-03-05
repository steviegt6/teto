using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Teto.Core.Data;
using Teto.Core.Models.Terraria;

const string db_path = "terraria-content.db";
var options = new DbContextOptionsBuilder<TerrariaContentDbContext>()
             .UseSqlite($"Data Source={db_path}")
             .Options;

using var db = new TerrariaContentDbContext(options);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

const string dir = "id-data";

Populate(db.Ammo, "AmmoIDs");
Populate(db.Dust, "DustIDs");
Populate(db.Buffs, "BuffIDs");
Populate(db.GlowMasks, "GlowMaskIDs");
Populate(db.Gore, "GoreIDs");
Populate(db.Items, "ItemIDs");
Populate(db.Mounts, "MountIDs");
Populate(db.Npcs, "NPCIDs");
Populate(db.Prefixes, "PrefixIDs");
Populate(db.Projectiles, "ProjectileIDs");
Populate(db.Sounds, "SoundIDs");
Populate(db.Walls, "WallIDs");

db.SaveChanges();

return;

static void Populate<T>(DbSet<T> set, string jsonName) where T : ContentId, new()
{
    var data = File.ReadAllText(Path.Combine(dir, jsonName + ".json"));
    var idData = JsonSerializer.Deserialize<IdCollection>(data)!;

    set.AddRange(
        idData.Ids.Select(
            x => new T
            {
                Id = x.Id,
                InternalName = x.InternalName,
                DisplayNameEnglish = x.DisplayName.Equals("No given name", StringComparison.OrdinalIgnoreCase) ? null : x.DisplayName,
                InfoLink = x.Link.Equals("No link", StringComparison.OrdinalIgnoreCase) ? null : x.Link,
            }
        )
    );
}

sealed class IdData(string id, string displayName, string link, string internalName)
{
    public string Id { get; set; } = id;

    public string DisplayName { get; set; } = displayName;

    public string Link { get; set; } = link;

    public string InternalName { get; set; } = internalName;
}

sealed class IdCollection
{
    public List<IdData> Ids { get; init; } = [];
}
