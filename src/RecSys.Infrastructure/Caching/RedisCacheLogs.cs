using Microsoft.Extensions.Logging;

namespace RecSys.Infrastructure.Caching;

internal static partial class RedisCacheLogs
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "[CACHE] Cache hit for user {UserId}. Count: {Count}")]
    public static partial void LogCacheHit(this ILogger logger, string userId, int count);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "[CACHE] Cache miss for user {UserId}")]
    public static partial void LogCacheMiss(this ILogger logger, string userId);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "[CACHE] Failed to deserialize recommendations for user {UserId}")]
    public static partial void LogDeserializationFailed(this ILogger logger, string userId, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "[CACHE] Cached {Count} recommendations for user {UserId} with TTL {Ttl}")]
    public static partial void LogRecommendationsCached(this ILogger logger, int count, string userId, TimeSpan ttl);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "[CACHE] Invalidated cache for user {UserId}")]
    public static partial void LogCacheInvalidated(this ILogger logger, string userId);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "[CACHE] Incremented popularity for item {ItemId} by {Score}. New score: {NewScore}")]
    public static partial void LogPopularityIncremented(this ILogger logger, string itemId, double score, double newScore);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "[CACHE] Failed to retrieve top popular items")]
    public static partial void LogPopularityRetrievalFailed(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Warning, Message = "[CACHE] Empty or default identifier passed to cache operation")]
    public static partial void LogEmptyIdentifier(this ILogger logger);

    [LoggerMessage(EventId = 9, Level = LogLevel.Warning, Message = "[CACHE] Attempted to cache null recommendations for user {UserId}")]
    public static partial void LogNullRecommendations(this ILogger logger, string userId);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "[CACHE] Failed to serialize recommendations for user {UserId}")]
    public static partial void LogSerializationFailed(this ILogger logger, string userId, Exception ex);
}
