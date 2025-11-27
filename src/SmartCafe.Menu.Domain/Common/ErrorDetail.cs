namespace SmartCafe.Menu.Domain.Common;

public sealed record ErrorDetail(string Message, string? Code = null, string? Field = null);
