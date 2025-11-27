namespace SmartCafe.Menu.Domain.Common;

public sealed class None
{
    public static readonly None Instance = new();
    private None() { }
}
