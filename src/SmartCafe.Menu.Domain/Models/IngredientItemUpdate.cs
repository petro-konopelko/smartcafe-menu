namespace SmartCafe.Menu.Domain.Models;

/// <summary>
/// Represents ingredient information for updating menu items.
/// </summary>
public record IngredientItemUpdate(string Name, bool IsExcludable);
