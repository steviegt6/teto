using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Teto.Core.Models.TmlTags;

/// <summary>
///     A tModLoader Discord tag.
/// </summary>
[PrimaryKey(nameof(OwnerSnowflake), nameof(TagName))]
[Index(nameof(OwnerSnowflake), nameof(TagName), IsUnique = true)]
public sealed class Tag
{
    /// <summary>
    ///     The Discord user snowflake for the user who created the tag.
    /// </summary>
    [Key]
    [Required]
    [Column(nameof(OwnerSnowflake))]
    public required ulong OwnerSnowflake { get; init; }

    /// <summary>
    ///     The name of the tag.
    /// </summary>
    [Key]
    [Required]
    [Column(nameof(TagName))]
    [MaxLength(32)]
    public required string TagName { get; init; }

    /// <summary>
    ///     The message contents of the tag.  May contain raw Markdown.
    /// </summary>
    [Required]
    [MaxLength(4096)]
    public required string Message { get; init; }

    /// <summary>
    ///     Whether this tag is globally accessible; that is, whether it may be
    ///     looked up without the corresponding user.
    /// </summary>
    [Required]
    public required bool IsGlobal { get; init; }
}
