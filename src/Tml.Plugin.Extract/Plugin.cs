using Microsoft.Extensions.DependencyInjection;
using Teto.Discord.Framework;
using Tml.Plugin.Extract.Services;

namespace Tml.Plugin.Extract;

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
