using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Bot.Services;
using Tomat.Teto.Framework;

namespace Tomat.Teto.Bot;

public sealed class HostPlugin : PluginDescription
{
    public override string UniqueName => "teto.bot-host";

    public override string Author => "tomat";

    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);

        // Ran on startup, handles initializing and running the bot.
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<BotStartService>();
    }
}
