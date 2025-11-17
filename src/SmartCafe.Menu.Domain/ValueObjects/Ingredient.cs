namespace SmartCafe.Menu.Domain.ValueObjects;

public class Ingredient
{
    public required string Name { get; init; }
    public bool IsExcludable { get; init; }
    public bool IsIncludable { get; init; }
}
