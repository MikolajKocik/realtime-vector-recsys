using MediatR;
using RecSys.Application.Abstractions.Output;
using RecSys.Domain.Enums;

namespace RecSys.Application.Features.Telemetry;

public sealed record TrackInteractionCommand(
    string UserId,
    string ItemId,
    InteractionType InteractionType,
    string? CorrelationId = null,
    string? ApiVersion = null,
    string? UserAgent = null
) : IRequest<Result>;
