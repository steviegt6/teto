namespace Tomat.Teto.Plugin.Tml.Tag.Models;

public readonly record struct TmlTagIdentity(ulong Owner, string Name)
{
    public string OwnerString { get; } = Owner.ToString();
}
