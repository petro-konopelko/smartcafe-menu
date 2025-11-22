namespace SmartCafe.Menu.Domain.Exceptions;

public class CafeNotFoundException(Guid cafeId)
    : Exception($"Cafe with ID {cafeId} was not found.")
{
    public Guid CafeId { get; } = cafeId;
}
