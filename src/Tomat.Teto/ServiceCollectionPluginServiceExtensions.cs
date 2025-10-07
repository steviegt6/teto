using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;

namespace Tomat.Teto;

public static class ServiceCollectionPluginServiceExtensions
{
    public static IServiceCollection AddBotPlugin<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TPluginDescription>(this IServiceCollection services)
        where TPluginDescription : PluginDescription, new()
    {
        var plugin = new BotPlugin(new TPluginDescription(), typeof(TPluginDescription).Assembly);
        {
            services.AddSingleton(plugin);
        }

        plugin.Description.AddServices(services);

        return services;
    }
}
