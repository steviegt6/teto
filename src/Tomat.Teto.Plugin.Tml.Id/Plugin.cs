using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;
using Tomat.Teto.Plugin.Tml.Id.Services;

namespace Tomat.Teto.Plugin.Tml.Id;

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
