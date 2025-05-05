namespace Tomat.Teto.Bot.DependencyInjection.Models;

public enum ServiceAdditionStatus
{
    Added,
    Replaced,
    AlreadyPresent,
}

public readonly record struct ServiceAdditionResult(ServiceAdditionStatus Status)
{
    public bool Successful => Status != ServiceAdditionStatus.AlreadyPresent;
}

public enum ServiceRemovalStatus
{
    WasNotPresent,
    Removed,
    Failed,
}

public readonly record struct ServiceRemovalResult(ServiceRemovalStatus Status)
{
    public bool Successful => Status != ServiceRemovalStatus.Failed;
}