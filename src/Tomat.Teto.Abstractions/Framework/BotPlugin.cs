using System.Reflection;

namespace Tomat.Teto.Framework;

/// <summary>
///     A known plugin.
/// </summary>
public record BotPlugin(PluginDescription Description, Assembly Assembly);
