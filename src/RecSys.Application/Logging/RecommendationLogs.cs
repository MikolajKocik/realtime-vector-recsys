using Microsoft.Extensions.Logging;

namespace RecSys.Application.Logging;

/// <summary>
/// Source-generated high-performance log messages for recommendation queries and cache operations.
/// </summary>
internal static partial class RecommendationLogs
{
    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Warning,
        Message = "GetRecommendations rejected: user identifier cannot be empty.")]
    public static partial void LogEmptyQueryUserId(this ILogger logger);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Warning,
        Message = "GetRecommendations rejected: TopK must be positive, received {TopK}.")]
    public static partial void LogInvalidTopK(this ILogger logger, int topK);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Information,
        Message = "Serving recommendations for user '{UserId}' from Redis cache. Items count: {Count}.")]
    public static partial void LogServingFromCache(this ILogger logger, string userId, int count);

    [LoggerMessage(
        EventId = 4004,
        Level = LogLevel.Information,
        Message = "Serving cold-start recommendations for user '{UserId}'. Items count: {Count}.")]
    public static partial void LogServingColdStart(this ILogger logger, string userId, int count);

    [LoggerMessage(
        EventId = 4005,
        Level = LogLevel.Information,
        Message = "Calculated HNSW recommendations for user '{UserId}' via pgvector. Candidates: {CandidatesCount}, Ranked: {RankedCount}.")]
    public static partial void LogServingHnsw(this ILogger logger, string userId, int candidatesCount, int rankedCount);

    [LoggerMessage(
        EventId = 4006,
        Level = LogLevel.Error,
        Message = "Failed to calculate recommendations for user '{UserId}'.")]
    public static partial void LogRecommendationError(this ILogger logger, Exception exception, string userId);
}
