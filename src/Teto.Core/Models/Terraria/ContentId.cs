using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Teto.Core.Models.Terraria;

/// <summary>
///     Models a piece of Terraria content identifiable by its internal numeric
///     ID.
/// </summary>
[PrimaryKey(nameof(Id))]
public abstract class ContentId
{
    /// <summary>
    ///     The numeric ID of this content.
    /// </summary>
    [Key]
    [Required]
    public string Id { get; init; } = "<no id>";

    /// <summary>
    ///     The internal string ID, usually indexable by an <c>IdDictionary</c>
    ///     in the Terraria source, but just corresponds to the constant field
    ///     with its value.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string InternalName { get; init; } = "<no name>";

    /// <summary>
    ///     The English display name, if applicable.
    /// </summary>
    [MaxLength(128)]
    public string? DisplayNameEnglish { get; init; }

    /// <summary>
    ///     A link to a place to read about the content, often an official wiki
    ///     page.
    /// </summary>
    [MaxLength(512)]
    public string? InfoLink { get; init; }
}

public sealed class AmmoId : ContentId;

public sealed class BuffId : ContentId;

public sealed class DustId : ContentId;

public sealed class GlowMaskId : ContentId;

public sealed class GoreId : ContentId;

public sealed class ItemId : ContentId;

public sealed class MountId : ContentId;

public sealed class NpcId : ContentId;

public sealed class PrefixId : ContentId;

public sealed class ProjectileId : ContentId;

public sealed class SoundId : ContentId;

public sealed class WallId : ContentId;
