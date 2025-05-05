using System;

namespace Tomat.Teto.Bot.DependencyInjection.Models;

public readonly record struct ServiceAdditionRequest(Type ServiceType, Type RegistrationType, Func<object>? Factory, bool Replace);

public readonly record struct ServiceRemovalRequest(Type RegistrationType, bool PropagateParents);

public readonly record struct ServiceRetrievalRequest(Type RegistrationType);