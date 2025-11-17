namespace SmartCafe.Menu.Domain.Entities;

public class Section
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid MenuId { get; init; }
    public Menu Menu { get; init; } = null!;
    public required string Name { get; init; }
    public int DisplayOrder { get; set; }
    public TimeSpan? AvailableFrom { get; init; }
    public TimeSpan? AvailableTo { get; init; }
    public ICollection<MenuItem> Items { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}
