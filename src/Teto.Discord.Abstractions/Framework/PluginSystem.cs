using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Teto.Discord.Framework;

// TODO: It'd be great to scrap this system in favor of something that supports
//       more inversion of control, but I think it works well enough for now?

/// <summary>
///     A known plugin.
/// </summary>
public record BotPlugin(PluginDescription Description, Assembly Assembly);

/// <summary>
///     The abstract description of a plugin, providing basic information and vital hooks.
/// </summary>
public abstract class PluginDescription
{
    /// <summary>
    ///     The unique identifier for this plugin.
    /// </summary>
    public abstract string UniqueName { get; }

    /// <summary>
    ///     The author string.
    /// </summary>
    public abstract string Author { get; }

    public virtual void AddServices(IServiceCollection services) { }
}
