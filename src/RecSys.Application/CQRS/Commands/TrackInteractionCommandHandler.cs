using System.Text.Json;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RecSys.Application.Abstractions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.Features.Telemetry;
using RecSys.Application.Logging;
using RecSys.Domain.Entities;
using RecSys.Domain.Enums;

namespace RecSys.Application.CQRS.Commands;

/// <summary>
/// Handles user telemetry commands by validating input, enriching with server metadata,
/// and dispatching the interaction event to the asynchronous message queue.
/// </summary>
public sealed class TrackInteractionCommandHandler(
    IInteractionEventQueue interactionEventQueue,
    TimeProvider timeProvider,
    ILogger<TrackInteractionCommandHandler> logger,
    INpgSqlProvider npgSqlProvider
) : IRequestHandler<TrackInteractionCommand, Result>
{
    private readonly IInteractionEventQueue _interactionEventQueue = interactionEventQueue;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<TrackInteractionCommandHandler> _logger = logger;
    private readonly INpgSqlProvider _npgSqlProvider = npgSqlProvider;

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

        if (!Enum.IsDefined<InteractionType>(request.InteractionType))
        {
            _logger.LogInteractionBadRequest(request.InteractionType);
            return new ErrorMessage(
                "Telemetry.InvalidInteractionTypeValue",
                ErrorType.BadRequest,
                "Interaction type value has not valid content.");
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

        try
        {
            await _interactionEventQueue.EnqueueAsync(userEvent, cancellationToken);

            _logger.LogInteractionAccepted(request.UserId, request.ItemId, request.InteractionType);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogRabbitMqFallbackToOutbox(ex, request.UserId, request.ItemId);

            var outboxMessage = new OutboxMessage(
                eventType: nameof(UserInteractedEvent),
                payloadJson: JsonSerializer.Serialize(userEvent)
            );

            const string sql = """
                INSERT INTO recsys."OutboxMessages" ("Id", "EventType", "PayloadJson", "OccurredAt")
                VALUES (@Id, @EventType, @PayloadJson, @OccurredAt);
            """;

            using var connection = _npgSqlProvider.CreateConnection();
            await connection.ExecuteAsync(new CommandDefinition(
                commandText: sql,
                parameters: outboxMessage,
                cancellationToken: cancellationToken)
            );

            return Result.Success();
        }
    }
}
