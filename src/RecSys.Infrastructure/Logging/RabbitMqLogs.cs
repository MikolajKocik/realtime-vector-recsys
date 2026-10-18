using Microsoft.Extensions.Logging;
using RecSys.Domain.Enums;

namespace RecSys.Infrastructure.Logging;

/// <summary>
/// Source-generated high-performance log messages for RabbitMQ operations.
/// </summary>
internal static partial class RabbitMqLogs
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "RabbitMQ topology initialized. Exchange: '{Exchange}', Queue: '{Queue}', DLX: '{DlxQueue}'")]
    public static partial void LogTopologyInitialized(this ILogger logger, string exchange, string queue, string dlxQueue);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Debug,
        Message = "Enqueued interaction event for User '{UserId}', Item '{ItemId}', Type: {InteractionType}")]
    public static partial void LogEventEnqueued(this ILogger logger, string userId, string itemId, InteractionType interactionType);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Warning,
        Message = "Malformed interaction payload received on delivery tag {DeliveryTag}. Forwarding to DLX.")]
    public static partial void LogMalformedPayload(this ILogger logger, ulong deliveryTag);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Error,
        Message = "Failed to deserialize interaction event on delivery tag {DeliveryTag}. Forwarding to DLX.")]
    public static partial void LogDeserializationError(this ILogger logger, Exception exception, ulong deliveryTag);
}
