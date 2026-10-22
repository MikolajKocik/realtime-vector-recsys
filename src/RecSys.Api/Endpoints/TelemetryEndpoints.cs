using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecSys.Api.Extensions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.CQRS.Commands;
using RecSys.Domain.Enums;

namespace RecSys.Api.Endpoints;

public static class TelemetryEndpoints
{

    public static IEndpointRouteBuilder MapTelemetryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/telemetry").WithTags("Telemetry");

        group.MapPost("/interaction", async (
            [FromBody] TrackInteractionRequest request,
            ISender sender,
            HttpContext context,
            CancellationToken ct) =>
        {
            string? correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            string? userAgent = context.Request.Headers.UserAgent.ToString();

            var command = new TrackInteractionCommand(
                UserId: request.UserId,
                ItemId: request.ItemId,
                InteractionType: request.InteractionType,
                CorrelationId: correlationId,
                ApiVersion: "1.0",
                UserAgent: userAgent
            );

            Result result = await sender.Send(command, ct);

            if (!result.IsSuccess && result.Error is not null)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Accepted();
        })
        .WithName("TrackInteraction")
        .Produces(StatusCodes.Status202Accepted)
        .Produces<ErrorMessage>(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record TrackInteractionRequest(
    string UserId,
    string ItemId,
    InteractionType InteractionType
);