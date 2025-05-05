using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Tomat.Teto.Bot.DependencyInjection.Models;

namespace Tomat.Teto.Bot.DependencyInjection;

public static class ServiceActivator
{
    private readonly record struct ServiceTypeOrServiceProvider(Type? Type)
    {
        public bool IsProvider => Type is null;
    }

    private readonly record struct ServiceDescription(ServiceTypeOrServiceProvider TypeOrProvider, bool Optional);

    public static object CreateService(IServiceProvider provider, in ServiceAdditionRequest request)
    {
        if (!request.ServiceType.IsAssignableTo(request.RegistrationType))
        {
            throw new ArgumentException("The registration type must be assignable to the service type.");
        }

        if (typeof(IService).IsAssignableFrom(request.ServiceType) && request.Factory is not null)
        {
            throw new ArgumentException("IService types cannot have a factory.");
        }

        var service = ActivateType(request.ServiceType, request.Factory, out var needsConstructor);

        PopulateObject(provider, service);

        if (needsConstructor)
        {
            HandlePreConstruction(provider, service);
            ConstructObject(provider, service);
            HandlePostConstruction(provider, service);
        }

        return service;
    }

    private static object ActivateType(Type typeToActivate, Func<object>? factory, out bool needsConstructor)
    {
        if (factory is not null)
        {
            var instance = factory();
            if (!typeToActivate.IsInstanceOfType(instance))
            {
                throw new ArgumentException("The factory must return an instance of the registration type.");
            }

            needsConstructor = false;
            return instance;
        }
        else
        {
            var instance = RuntimeHelpers.GetUninitializedObject(typeToActivate);
            needsConstructor = true;
            return instance;
        }
    }

    private static void PopulateObject(IServiceProvider provider, object value)
    {
        ServiceDescriptionsFromType(value.GetType(), out var fields, out var properties);

        foreach (var field in fields)
        {
            if (field.Value.TypeOrProvider.IsProvider)
            {
                field.Key.SetValue(value, provider);
            }
            else
            {
                provider.TryGetService(new ServiceRetrievalRequest(field.Value.TypeOrProvider.Type!), out var service);

                if (service is null && !field.Value.Optional)
                {
                    throw new InvalidOperationException("A required service was not found.");
                }

                field.Key.SetValue(value, service);
            }
        }

        foreach (var property in properties)
        {
            if (property.Value.TypeOrProvider.IsProvider)
            {
                property.Key.SetValue(value, provider);
            }
            else
            {
                provider.TryGetService(new ServiceRetrievalRequest(property.Value.TypeOrProvider.Type!), out var service);

                if (service is null && !property.Value.Optional)
                {
                    throw new InvalidOperationException("A required service was not found.");
                }

                property.Key.SetValue(value, service);
            }
        }
    }

    private static void ConstructObject(IServiceProvider provider, object value)
    {
        var type = value.GetType();
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (constructor.GetCustomAttribute<PreferredServiceConstructorAttribute>() is null)
            {
                continue;
            }

            RunMethod(provider, value, constructor);
            return;
        }

        // If no preferred constructor was found, just use the one with no
        // parameters.
        var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);

        // If no parameterless constructor was found, throw an exception.
        if (parameterlessConstructor is null)
        {
            throw new InvalidOperationException("No suitable constructor was found for the service.");
        }

        RunMethod(provider, value, parameterlessConstructor);
    }

    private static void InvokeServiceHooks(IServiceProvider provider, object service, ServiceHook hook)
    {
        var type = service.GetType();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (method.GetCustomAttribute<ServiceHookAttribute>() is not { } hookAttr)
            {
                continue;
            }

            if (hookAttr.Hook != hook)
            {
                continue;
            }

            RunMethod(provider, service, method);
        }
    }

    private static void RunMethod(IServiceProvider provider, object value, MethodBase method)
    {
        ServiceDescriptionsFromMethod(method, out var parameterDescriptions);
        var parameters = parameterDescriptions.Select(x =>
            {
                if (x.TypeOrProvider.IsProvider)
                {
                    return provider;
                }

                provider.TryGetService(new ServiceRetrievalRequest(x.TypeOrProvider.Type!), out var service);

                if (service is null && !x.Optional)
                {
                    throw new InvalidOperationException("A required service was not found.");
                }

                return service;
            }
        );

        method.Invoke(value, parameters.ToArray());
    }

    public static void HandlePreConstruction(IServiceProvider provider, object service)
    {
        InvokeServiceHooks(provider, service, ServiceHook.PreConstruct);
    }

    public static void HandlePostConstruction(IServiceProvider provider, object service)
    {
        InvokeServiceHooks(provider, service, ServiceHook.PostConstruct);
    }

    public static void HandleServiceRemoval(IServiceProvider provider, object service)
    {
        InvokeServiceHooks(provider, service, ServiceHook.OnRemoval);
    }

    private static void ServiceDescriptionsFromType(Type type, out Dictionary<FieldInfo, ServiceDescription> fields, out Dictionary<PropertyInfo, ServiceDescription> properties)
    {
        fields = new Dictionary<FieldInfo, ServiceDescription>();
        properties = new Dictionary<PropertyInfo, ServiceDescription>();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.IsStatic)
            {
                continue;
            }

            var optional = field.GetCustomAttribute<OptionalServiceAttribute>() is not null;
            if (field.GetCustomAttribute<ServiceAttribute>() is not { } serviceAttr)
            {
                if (field.GetCustomAttribute<ServiceProviderAttribute>() is null)
                {
                    continue;
                }

                fields.Add(field, new ServiceDescription(new ServiceTypeOrServiceProvider(null), optional));
                continue;
            }

            fields.Add(field, new ServiceDescription(new ServiceTypeOrServiceProvider(serviceAttr.RegistrationType ?? field.FieldType), optional));
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (property.GetMethod is null || property.SetMethod is null)
            {
                continue;
            }

            var optional = property.GetCustomAttribute<OptionalServiceAttribute>() is not null;
            if (property.GetCustomAttribute<ServiceAttribute>() is not { } serviceAttr)
            {
                if (property.GetCustomAttribute<ServiceProviderAttribute>() is null)
                {
                    continue;
                }

                properties.Add(property, new ServiceDescription(new ServiceTypeOrServiceProvider(null), optional));
                continue;
            }

            properties.Add(property, new ServiceDescription(new ServiceTypeOrServiceProvider(serviceAttr.RegistrationType ?? property.PropertyType), optional));
        }
    }

    private static void ServiceDescriptionsFromMethod(MethodBase method, out List<ServiceDescription> parameters)
    {
        parameters = [];

        foreach (var parameter in method.GetParameters())
        {
            var optional = parameter.GetCustomAttribute<OptionalServiceAttribute>() is not null;
            if (parameter.GetCustomAttribute<ServiceAttribute>() is not { } serviceAttr)
            {
                if (parameter.GetCustomAttribute<ServiceProviderAttribute>() is null)
                {
                    parameters.Add(new ServiceDescription(new ServiceTypeOrServiceProvider(parameter.ParameterType), optional));
                    continue;
                }

                parameters.Add(new ServiceDescription(new ServiceTypeOrServiceProvider(null), optional));
                continue;
            }

            parameters.Add(new ServiceDescription(new ServiceTypeOrServiceProvider(serviceAttr.RegistrationType ?? parameter.ParameterType), optional));
        }
    }
}