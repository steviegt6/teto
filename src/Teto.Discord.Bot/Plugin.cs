using Microsoft.Extensions.DependencyInjection;
using Teto.Discord.Bot.Services;
using Teto.Discord.Framework;

namespace Teto.Discord.Bot;

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
