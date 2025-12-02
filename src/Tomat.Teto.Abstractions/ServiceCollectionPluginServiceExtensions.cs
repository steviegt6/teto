using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;

namespace Tomat.Teto;

/// <summary>
///     Extension methods for adding and removing plugins to an
///     <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionPluginServiceExtensions
{
    
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds a new plugin of type
        ///     <typeparamref name="TPluginDescription"/> to the service provider.
        /// </summary>
        public IServiceCollection AddBotPlugin<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TPluginDescription
        >()
            where TPluginDescription : PluginDescription, new()
        {
            ArgumentNullException.ThrowIfNull(services);

            var plugin = new BotPlugin(new TPluginDescription(), typeof(TPluginDescription).Assembly);
            {
                services.AddSingleton(plugin);
            }

            plugin.Description.AddServices(services);

            return services;
        }
    }
}
