using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tomat.Teto.Framework;

namespace Tomat.Teto.Bot.Services;

internal sealed class InteractionHandler : IHostedService
{
    private readonly DiscordSocketClient client;
    private readonly InteractionService interactions;
    private readonly IServiceProvider services;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider services,
        ILogger<InteractionService> logger
    )
    {
        this.client = client;
        this.interactions = interactions;
        this.services = services;

        interactions.Log += logger.CreateDefaultLogHandler();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Ready += async () =>
        {
            await interactions.RegisterCommandsGloballyAsync(deleteMissing: true);
        };

        client.InteractionCreated += HandleInteraction;
        interactions.InteractionExecuted += HandleInteractionExecute;

        foreach (var plugin in services.GetServices<BotPlugin>())
        {
            await interactions.AddModulesAsync(plugin.Assembly, services);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        interactions.Dispose();
        return Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(client, interaction);
            var result = await interactions.ExecuteCommandAsync(context, services);

            if (!result.IsSuccess)
            {
                // TODO
                /*switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        break;

                    default:
                        break;
                }*/
            }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync()
                                 .ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }

    private static Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess)
        {
            return Task.CompletedTask;
        }

        // TODO
        /*
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                // implement
                break;

            default:
                break;
        }
        */

        return Task.CompletedTask;
    }
}
