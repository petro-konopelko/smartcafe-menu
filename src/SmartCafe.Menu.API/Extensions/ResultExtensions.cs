using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return Results.Ok(result.EnsureValue());
        }

        var error = result.EnsureError();
        return error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new
            {
                error
            }),
            ErrorType.Validation => Results.BadRequest(new { error }),
            ErrorType.Conflict => Results.Conflict(new { error }),
            _ => throw new InvalidOperationException("Unhandled ErrorType mapping. Register a mapping for this status.")
        };
    }

    public static IResult ToApiResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        var error = result.EnsureError();
        return error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { error }),
            ErrorType.Validation => Results.BadRequest(new { error }),
            ErrorType.Conflict => Results.Conflict(new { error }),
            _ => throw new InvalidOperationException("Unhandled ErrorType mapping. Register a mapping for this status.")
        };
    }

    public static IResult ToCreatedResult<T>(this Result<T> result, Func<T, string> locationFactory)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(locationFactory);

        if (result.IsSuccess)
        {
            return Results.Created(locationFactory(result.Value!), result.Value);
        }
        return result.ToApiResult();
    }

    public static IResult ToNoContentResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }
        return result.ToApiResult();
    }
}
