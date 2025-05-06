namespace Tomat.Teto.Bot.Models;

public readonly record struct TmlTagIdentity(ulong Owner, string Name)
{
    public string OwnerString { get; } = Owner.ToString();
}

public readonly record struct TmlTag(TmlTagIdentity Identity, string Value, bool IsGlobal);