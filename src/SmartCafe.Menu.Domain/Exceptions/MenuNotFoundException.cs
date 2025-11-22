namespace SmartCafe.Menu.Domain.Exceptions;

public class MenuNotFoundException(Guid menuId)
    : Exception($"Menu with ID {menuId} was not found.")
{
    public Guid MenuId { get; } = menuId;
}
