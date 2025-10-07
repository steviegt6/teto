using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;
using Tomat.Teto.Plugin.Tml.Tag.Services;

namespace Tomat.Teto.Plugin.Tml.Tag;

public sealed class TmlTagPlugin : PluginDescription
{
    public override string UniqueName => "teto.tml.id";

    public override string Author => "tomat";

    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);

        services.AddSingleton<TmlTagService>();
    }
}
