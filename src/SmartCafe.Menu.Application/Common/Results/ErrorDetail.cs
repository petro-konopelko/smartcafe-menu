namespace SmartCafe.Menu.Application.Common.Results;

public sealed record ErrorDetail(string Message, string? Code = null, string? Field = null);
