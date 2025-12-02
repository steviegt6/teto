using Microsoft.Extensions.DependencyInjection;

namespace Tomat.Teto.Framework;

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
