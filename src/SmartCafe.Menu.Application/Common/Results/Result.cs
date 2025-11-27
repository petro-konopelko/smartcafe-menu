namespace SmartCafe.Menu.Application.Common.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    private protected Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private protected Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}

/// <summary>
/// Result without payload. Inherits from Result&lt;None&gt;.
/// Use Result.Success() for success without return value.
/// </summary>
public sealed class Result : Result<None>
{
    private Result(None value) : base(value) { }
    private Result(Error error) : base(error) { }

    public static Result Success() => new(None.Instance);
    public new static Result Failure(Error error) => new(error);
}
