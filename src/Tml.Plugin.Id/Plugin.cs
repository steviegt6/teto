using Microsoft.Extensions.DependencyInjection;
using Teto.Discord.Framework;
using Tml.Plugin.Id.Services;

namespace Tml.Plugin.Id;

public sealed class TmlIdPlugin : PluginDescription
{
    public override string UniqueName => "teto.tml.tag";

    public override string Author => "tomat, olihh";

    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);

        services.AddSingleton<TmlIdService>();
    }
}
