namespace SmartCafe.Menu.Domain.Common;

public sealed class Error
{
    public ErrorType Type { get; }
    public IReadOnlyList<ErrorDetail> Details { get; }

    private Error(ErrorType type, IReadOnlyList<ErrorDetail> details)
    {
        Type = type;
        Details = details;
    }

    public static Error Create(ErrorType type, params ErrorDetail[] details) =>
        new(type, details);

    public static Error Create(ErrorType type, IEnumerable<ErrorDetail> details) =>
        new(type, details.ToList());

    public static Error NotFound(string message, string? code = null) =>
        new(ErrorType.NotFound, new[] { new ErrorDetail(message, code) });

    public static Error Validation(params ErrorDetail[] details) =>
        new(ErrorType.Validation, details);

    public static Error Validation(IEnumerable<ErrorDetail> details) =>
        new(ErrorType.Validation, details.ToList());

    public static Error Conflict(string message, string? code = null) =>
        new(ErrorType.Conflict, new[] { new ErrorDetail(message, code) });
}
