using System.Text.Json.Serialization;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Domain.ValueObjects;

public class Ingredient
{
    public string Name { get; }
    public bool IsExcludable { get; }

    // Required for JSON deserialization and EF Core materialization
    [JsonConstructor]
    private Ingredient(string name, bool isExcludable)
    {
        Name = name;
        IsExcludable = isExcludable;
    }

    internal static Result<Ingredient> Create(string name, bool isExcludable)
    {
        var trimmedName = name?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return Result<Ingredient>.Failure(Error.Validation(new ErrorDetail("Ingredient name is required", ItemErrorCodes.IngredientNameRequired)));
        }

        return Result<Ingredient>.Success(new Ingredient(trimmedName, isExcludable));
    }
}
