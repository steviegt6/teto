using Microsoft.EntityFrameworkCore;
using Teto.Core.Models.TmlTags;

namespace Teto.Core.Data;

public sealed class DiscordTagsDbContext(DbContextOptions<DiscordTagsDbContext> options) : DbContext(options)
{
    public DbSet<Tag> Tags => Set<Tag>();
}
