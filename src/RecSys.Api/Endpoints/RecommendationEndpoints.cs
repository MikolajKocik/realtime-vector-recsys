using Asp.Versioning;
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
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/recommendations")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new ApiVersion(1, 0))
            .WithTags("Recommendations");

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
