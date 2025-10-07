using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;
using Tomat.Teto.Plugin.Tml.Extract.Services;

namespace Tomat.Teto.Plugin.Tml.Extract;

public sealed class TmlExtractPlugin : PluginDescription
{
    public override string UniqueName => "teto.tml.extract";

    public override string Author => "tomat";

    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);

        services.AddSingleton<TmlExtractService>();
    }
}
