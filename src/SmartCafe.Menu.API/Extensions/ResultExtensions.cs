using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        if (result.Error is null)
        {
            throw new InvalidOperationException("The result is failure. Please specify the error.");
        }

        return result.Error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { error = result.Error }),
            ErrorType.Validation => Results.BadRequest(new { error = result.Error }),
            ErrorType.Conflict => Results.Conflict(new { error = result.Error }),
            _ => throw new InvalidOperationException("Unhandled ErrorType mapping. Register a mapping for this status.")
        };
    }

    public static IResult ToCreatedResult<T>(this Result<T> result, Func<T, string> locationFactory)
    {
        if (result.IsSuccess)
        {
            return Results.Created(locationFactory(result.Value!), result.Value);
        }
        return result.ToApiResult();
    }

    public static IResult ToNoContentResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }
        return result.ToApiResult();
    }
}
