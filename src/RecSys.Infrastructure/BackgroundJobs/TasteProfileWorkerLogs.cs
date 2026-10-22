using Microsoft.Extensions.Logging;

namespace RecSys.Infrastructure.BackgroundJobs;

internal static partial class TasteProfileWorkerLogs
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "[WORKER] TasteProfileWorker started listening for interactions.")]
    public static partial void LogWorkerStarted(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "[WORKER] Error processing user interaction event")]
    public static partial void LogProcessingError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "[WORKER] Catalog item {ItemId} not found for user interaction")]
    public static partial void LogCatalogItemNotFound(this ILogger logger, string itemId);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "[WORKER] Failed to update taste profile for user {UserId}")]
    public static partial void LogProfileUpdateFailed(this ILogger logger, string userId, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "[WORKER] Successfully updated taste profile for user {UserId} based on item {ItemId}")]
    public static partial void LogProfileUpdated(this ILogger logger, string userId, string itemId);
}
