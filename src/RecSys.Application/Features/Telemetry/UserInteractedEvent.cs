using RecSys.Domain.Enums;

namespace RecSys.Application.Features.Telemetry;

public sealed record UserInteractedEvent(
    string UserId,
    string ItemId,
    InteractionType InteractionType,
    DateTimeOffset OccurredAt);
