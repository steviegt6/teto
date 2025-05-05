using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using Tomat.Teto.Bot.DependencyInjection;
using Tomat.Teto.Bot.DependencyInjection.Models;

using IServiceProvider = Tomat.Teto.Bot.DependencyInjection.IServiceProvider;

namespace Tomat.Teto.Bot.Services;

public sealed class InteractionHandler : IService
{
    private sealed class StupidWrapper(IServiceProvider services) : System.IServiceProvider
    {
        private sealed class Stupid(System.IServiceProvider s) : IServiceScopeFactory
        {
            private sealed class Stupid2(System.IServiceProvider s) : IServiceScope
            {
                public System.IServiceProvider ServiceProvider { get; } = s;

                public void Dispose() { }
            }

            public IServiceScope CreateScope()
            {
                return new Stupid2(s);
            }
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceScopeFactory))
            {
                return new Stupid(this);
            }

            return services.TryGetService(new ServiceRetrievalRequest(serviceType), out var value) ? value : null;
        }
    }

    [Service]
    public DiscordSocketClient Client { get; set; }

    [Service]
    public InteractionService Handler { get; set; }

    [ServiceProvider]
    public IServiceProvider Services { get; set; }

    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        Client.Ready += ReadyAsync;
        Handler.Log += LogAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await Handler.AddModulesAsync(Assembly.GetEntryAssembly(), new StupidWrapper(Services));

        // Process the InteractionCreated payloads to execute Interactions commands
        Client.InteractionCreated += HandleInteraction;

        // Also process the result of the command execution.
        Handler.InteractionExecuted += HandleInteractionExecute;
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        // Register the commands globally.
        // alternatively you can use _handler.RegisterCommandsGloballyAsync() to register commands to a specific guild.
        await Handler.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(Client, interaction);

            // Execute the incoming command.
            var result = await Handler.ExecuteCommandAsync(context, new StupidWrapper(Services));

            // Due to async nature of InteractionFramework, the result here may always be success.
            // That's why we also need to handle the InteractionExecuted event.
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;

                    default:
                        break;
                }
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess)
        {
            return Task.CompletedTask;
        }

        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                // implement
                break;

            default:
                break;
        }

        return Task.CompletedTask;
    }
}