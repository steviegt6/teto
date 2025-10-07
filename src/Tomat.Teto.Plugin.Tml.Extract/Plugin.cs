using System;
using CatBox.NET;
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
        
        services.AddCatBoxServices(
            setup =>
            {
                setup.CatBoxUrl = new Uri("https://catbox.moe/user/api.php");
                setup.LitterboxUrl = new Uri("https://litterbox.catbox.moe/resources/internals/api.php");
            }
        );
    }
}
