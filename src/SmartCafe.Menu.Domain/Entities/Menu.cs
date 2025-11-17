namespace SmartCafe.Menu.Domain.Entities;

public class Menu
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid CafeId { get; init; }
    public Cafe Cafe { get; init; } = null!;
    public required string Name { get; init; }
    public bool IsActive { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public List<Section> Sections { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
