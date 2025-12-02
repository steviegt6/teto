using Microsoft.Extensions.DependencyInjection;
using Teto.Discord.Framework;
using Tml.Plugin.Tag.Services;

namespace Tml.Plugin.Tag;

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
