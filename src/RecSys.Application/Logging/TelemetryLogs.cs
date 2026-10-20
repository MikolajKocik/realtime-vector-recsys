using Microsoft.Extensions.Logging;
using RecSys.Domain.Enums;

namespace RecSys.Application.Logging;

/// <summary>
/// Source-generated high-performance log messages for telemetry operations.
/// </summary>
internal static partial class TelemetryLogs
{
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Warning,
        Message = "TrackInteraction rejected: user identifier cannot be empty.")]
    public static partial void LogEmptyUserId(this ILogger logger);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Warning,
        Message = "TrackInteraction rejected: item identifier cannot be empty for user '{UserId}'.")]
    public static partial void LogEmptyItemId(this ILogger logger, string userId);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Information,
        Message = "TrackInteraction accepted: User '{UserId}' interacted with Item '{ItemId}', Type: {InteractionType}.")]
    public static partial void LogInteractionAccepted(this ILogger logger, string userId, string itemId, InteractionType interactionType);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Warning,
        Message = "TrackInteraction rejected: Invalid interaction type '{InteractionType}'.")]
    public static partial void LogInteractionBadRequest(this ILogger logger, InteractionType interactionType);

    [LoggerMessage(
        EventId = 3005,
        Level = LogLevel.Warning,
        Message = "RabbitMQ unavailable while enqueuing interaction for User '{UserId}', Item '{ItemId}'. Falling back to Outbox via Dapper.")]
    public static partial void LogRabbitMqFallbackToOutbox(this ILogger logger, Exception exception, string userId, string itemId);
}
