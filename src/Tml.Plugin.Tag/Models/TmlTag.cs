namespace Tml.Plugin.Tag.Models;

public readonly record struct TmlTag(TmlTagIdentity Identity, string Value, bool IsGlobal);