namespace SmartCafe.Menu.Domain.Entities;

public class Cafe
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public string? ContactInfo { get; init; }
    public DateTime CreatedAt { get; init; }
}
