using System.Reflection;

namespace Teto.Discord.Framework;

/// <summary>
///     A known plugin.
/// </summary>
public record BotPlugin(PluginDescription Description, Assembly Assembly);
