using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecSys.Api.Extensions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.CQRS.Queries;
using RecSys.Application.Features.Recommendations;

namespace RecSys.Api.Endpoints;

public static class RecommendationEndpoints
{
    public static IEndpointRouteBuilder MapRecommendationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recommendations").WithTags("Recommendations");

        group.MapGet("/{userId}", async (
            string userId,
            [FromQuery] int? topK,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetRecommendationsQuery(
                UserId: userId,
                TopK: topK ?? 5
            );

            Result<RecommendationResponseDto> result = await sender.Send(query, ct);

            if (!result.IsSuccess && result.Error is not null)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetRecommendations")
        .Produces<RecommendationResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
