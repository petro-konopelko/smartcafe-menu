namespace SmartCafe.Menu.API.Models.Requests.Cafes;

public record CreateCafeRequest(
    string Name,
    string? ContactInfo
);
