namespace SmartCafe.Menu.Domain.Common;

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

    public T EnsureValue()
    {
        return IsFailure || Value is null ? throw new InvalidOperationException("Cannot retrieve the value from a failed result.") : Value;
    }

    public Error EnsureError()
    {
        return IsSuccess || Error is null
            ? throw new InvalidOperationException("Cannot retrieve the error from a successful result.")
            : Error;
    }
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
