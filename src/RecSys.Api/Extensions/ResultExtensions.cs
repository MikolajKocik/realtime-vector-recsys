using RecSys.Application.Abstractions.Output;

namespace RecSys.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this ErrorMessage error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource Not Found",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Id }
            ),
            ErrorType.BadRequest => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Id }
            ),
            _ => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Internal Server Error",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Id }
            ),
        };
    }
}
