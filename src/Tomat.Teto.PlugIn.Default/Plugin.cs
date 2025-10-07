﻿using Microsoft.Extensions.DependencyInjection;
using Tomat.Teto.Framework;
using Tomat.Teto.Plugin.Default.Services;

namespace Tomat.Teto.Plugin.Default;

public sealed class DefaultPlugin : PluginDescription
{
    public override string UniqueName => "teto.default";

    public override string Author => "tomat, olihh";

    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);

        services.AddSingleton<MessageSelectService>();
        services.AddSingleton<PasteService>();
        services.AddSingleton<UptimeService>();
    }
}
