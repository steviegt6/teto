using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Tomat.Teto.Bot.DependencyInjection.Models;

namespace Tomat.Teto.Bot.DependencyInjection;

public interface IServiceProvider : IDisposable
{
    ServiceAdditionResult TryAddService(in ServiceAdditionRequest request);

    bool TryGetService(in ServiceRetrievalRequest request, [NotNullWhen(returnValue: true)] out object? value);

    ServiceRemovalResult TryRemoveService(in ServiceRemovalRequest request);
}

public sealed class ServiceProvider(IServiceProvider? parent = null) : IServiceProvider
{
    private static readonly ServiceAdditionResult added = new(ServiceAdditionStatus.Added);
    private static readonly ServiceAdditionResult replaced = new(ServiceAdditionStatus.Replaced);
    private static readonly ServiceAdditionResult already_present = new(ServiceAdditionStatus.AlreadyPresent);

    private static readonly ServiceRemovalResult was_not_present = new(ServiceRemovalStatus.WasNotPresent);
    private static readonly ServiceRemovalResult removed = new(ServiceRemovalStatus.Removed);
    private static readonly ServiceRemovalResult failed = new(ServiceRemovalStatus.Failed);


    private readonly Dictionary<Type, object> services = [];

    public ServiceAdditionResult TryAddService(in ServiceAdditionRequest request)
    {
        if (request.Factory is not null && typeof(IService).IsAssignableFrom(request.RegistrationType))
        {
            throw new ArgumentException("Cannot register a service with a factory if the service type is assignable from IService.", nameof(request));
        }

        if (!request.Replace)
        {
            return services.TryAdd(request.RegistrationType, ServiceActivator.CreateService(this, in request)) ? added : already_present;
        }

        var present = services.ContainsKey(request.RegistrationType);
        services[request.RegistrationType] = ServiceActivator.CreateService(this, in request);
        return present ? replaced : added;
    }

    public bool TryGetService(in ServiceRetrievalRequest request, [NotNullWhen(true)] out object? value)
    {
        if (parent?.TryGetService(in request, out value) ?? false)
        {
            return true;
        }

        return services.TryGetValue(request.RegistrationType, out value);
    }

    public ServiceRemovalResult TryRemoveService(in ServiceRemovalRequest request)
    {
        if (services.TryGetValue(request.RegistrationType, out var service))
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }

            ServiceActivator.HandleServiceRemoval(this, service);
            return services.Remove(request.RegistrationType) ? removed : failed;
        }

        if (!request.PropagateParents || parent is null)
        {
            return was_not_present;
        }

        return parent.TryGetService(new ServiceRetrievalRequest(request.RegistrationType), out _) ? parent.TryRemoveService(in request) : was_not_present;
    }

    public void Dispose()
    {
        parent?.Dispose();

        foreach (var service in services.Values)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        services.Clear();
    }
}

public static class ServiceProviderExtensions
{
    public static ServiceAdditionResult TryAddService(this IServiceProvider provider, Type serviceType, Type registrationType, Func<object>? factory = null, bool replace = false)
    {
        return provider.TryAddService(new ServiceAdditionRequest(serviceType, registrationType, factory, replace));
    }

    public static ServiceAdditionResult TryAddService<T>(this IServiceProvider provider, bool replace = false)
    {
        return provider.TryAddService(new ServiceAdditionRequest(typeof(T), typeof(T), null, replace));
    }

    public static ServiceAdditionResult TryAddService<T>(this IServiceProvider provider, T instance, bool replace = false) where T : notnull
    {
        return provider.TryAddService(new ServiceAdditionRequest(typeof(T), typeof(T), () => instance, replace));
    }

    public static ServiceAdditionResult TryAddService<TSource, TReal>(this IServiceProvider provider, bool replace = false) where TReal : TSource
    {
        return provider.TryAddService(new ServiceAdditionRequest(typeof(TReal), typeof(TSource), null, replace));
    }

    public static bool TryGetService(this IServiceProvider provider, Type registrationType, [NotNullWhen(returnValue: true)] out object? value)
    {
        return provider.TryGetService(new ServiceRetrievalRequest(registrationType), out value);
    }

    public static bool TryGetService<T>(this IServiceProvider provider, [NotNullWhen(returnValue: true)] out T? value)
    {
        var result = provider.TryGetService(new ServiceRetrievalRequest(typeof(T)), out var obj);
        value = (T?)obj;
        return result;
    }

    public static object ExpectService(this IServiceProvider provider, Type registrationType)
    {
        if (!provider.TryGetService(registrationType, out var value))
        {
            throw new InvalidOperationException($"Service of type {registrationType} was not found.");
        }

        return value;
    }

    public static T ExpectService<T>(this IServiceProvider provider)
    {
        if (!provider.TryGetService<T>(out var value))
        {
            throw new InvalidOperationException($"Service of type {typeof(T)} was not found.");
        }

        return value;
    }

    public static ServiceRemovalResult TryRemoveService(this IServiceProvider provider, Type registrationType, bool propagateParents = false)
    {
        return provider.TryRemoveService(new ServiceRemovalRequest(registrationType, propagateParents));
    }

    public static ServiceRemovalResult TryRemoveService<T>(this IServiceProvider provider, bool propagateParents = false)
    {
        return provider.TryRemoveService(new ServiceRemovalRequest(typeof(T), propagateParents));
    }
}