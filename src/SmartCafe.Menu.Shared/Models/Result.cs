namespace SmartCafe.Menu.Shared.Models;

/// <summary>
/// Result without payload.
/// Use Result.Success() for success without return value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    private protected Result(bool success, Error? error = null)
    {
        IsSuccess = success;
        Error = error;
    }
    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false, error);

    public Error EnsureError()
    {
        return IsSuccess || Error is null
            ? throw new InvalidOperationException("Cannot retrieve the error from a successful result.")
            : Error;
    }

    public void EnsureSuccess()
    {
        if (IsFailure)
        {
            throw new InvalidOperationException($"Operation failed. Message: {GetFailureMessage()}");
        }
    }

    private protected string GetFailureMessage()
    {
        return Error is null
            ? "Unknown error"
            : $"Error type: {Error.Type}, Message: {string.Join(", ", Error.Details.Select(d => d.Message))}";
    }
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true) { Value = value; }

    private Result(Error error) : base(false, error) { }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);

    public T EnsureValue()
    {
        return IsFailure || Value is null
            ? throw new InvalidOperationException($"Cannot retrieve the value from a failed result. Message: {GetFailureMessage()}")
            : Value;
    }
}


