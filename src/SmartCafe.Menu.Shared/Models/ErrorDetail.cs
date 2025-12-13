namespace SmartCafe.Menu.Shared.Models;

public sealed record ErrorDetail(string Message, string? Code = null, string? Field = null);
