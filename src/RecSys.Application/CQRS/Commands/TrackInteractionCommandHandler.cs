using MediatR;
using Microsoft.Extensions.Logging;
using RecSys.Application.Abstractions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.Features.Telemetry;
using RecSys.Application.Logging;

namespace RecSys.Application.CQRS.Commands;

/// <summary>
/// Handles user telemetry commands by validating input, enriching with server metadata,
/// and dispatching the interaction event to the asynchronous message queue.
/// </summary>
public sealed class TrackInteractionCommandHandler(
    IInteractionEventQueue interactionEventQueue,
    TimeProvider timeProvider,
    ILogger<TrackInteractionCommandHandler> logger
) : IRequestHandler<TrackInteractionCommand, Result>
{
    private readonly IInteractionEventQueue _interactionEventQueue = interactionEventQueue;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<TrackInteractionCommandHandler> _logger = logger;

    public async Task<Result> Handle(TrackInteractionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            _logger.LogEmptyUserId();
            return new ErrorMessage(
                "Telemetry.InvalidUserId",
                ErrorType.BadRequest,
                "User identifier cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.ItemId))
        {
            _logger.LogEmptyItemId(request.UserId);
            return new ErrorMessage(
                "Telemetry.InvalidItemId",
                ErrorType.BadRequest,
                "Item identifier cannot be empty.");
        }

        var userEvent = new UserInteractedEvent(
            UserId: request.UserId,
            ItemId: request.ItemId,
            InteractionType: request.InteractionType,
            OccurredAt: _timeProvider.GetUtcNow(),
            CorrelationId: request.CorrelationId,
            ApiVersion: request.ApiVersion,
            UserAgent: request.UserAgent
        );

        await _interactionEventQueue.EnqueueAsync(userEvent, cancellationToken);

        _logger.LogInteractionAccepted(request.UserId, request.ItemId, request.InteractionType);

        return Result.Success();
    }
}
