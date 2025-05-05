using System;

using JetBrains.Annotations;

namespace Tomat.Teto.Bot.DependencyInjection.Models;

// [MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class OptionalServiceAttribute : Attribute;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class PreferredServiceConstructorAttribute : Attribute;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ServiceAttribute(Type? registrationType = null, bool optional = false) : Attribute
{
    public Type? RegistrationType { get; } = registrationType;

    public bool Optional { get; } = optional;
}

public enum ServiceHook
{
    PreConstruct,
    PostConstruct,
    OnRemoval,
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public sealed class ServiceHookAttribute(ServiceHook hook) : Attribute
{
    public ServiceHook Hook { get; } = hook;
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ServiceProviderAttribute : Attribute;