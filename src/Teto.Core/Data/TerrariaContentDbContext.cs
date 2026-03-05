using Microsoft.EntityFrameworkCore;
using Teto.Core.Models.Terraria;

namespace Teto.Core.Data;

public sealed class TerrariaContentDbContext(DbContextOptions<TerrariaContentDbContext> options) : DbContext(options)
{
    public DbSet<AmmoId> Ammo => Set<AmmoId>();

    public DbSet<BuffId> Buffs => Set<BuffId>();

    public DbSet<DustId> Dust => Set<DustId>();

    public DbSet<GlowMaskId> GlowMasks => Set<GlowMaskId>();

    public DbSet<GoreId> Gore => Set<GoreId>();

    public DbSet<ItemId> Items => Set<ItemId>();

    public DbSet<MountId> Mounts => Set<MountId>();

    public DbSet<NpcId> Npcs => Set<NpcId>();

    public DbSet<PrefixId> Prefixes => Set<PrefixId>();

    public DbSet<ProjectileId> Projectiles => Set<ProjectileId>();

    public DbSet<SoundId> Sounds => Set<SoundId>();

    public DbSet<WallId> Walls => Set<WallId>();
}
