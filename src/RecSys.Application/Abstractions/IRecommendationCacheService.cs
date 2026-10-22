using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Provides caching operations for recommendations and real-time item popularity tracking.
/// </summary>
public interface IRecommendationCacheService
{
    /// <summary>
    /// Retrieves cached recommendations for a specified user, if present.
    /// </summary>
    /// <param name="userId">Unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of cached ranked results, or null if cache miss.</returns>
    Task<IReadOnlyList<RankedResult>?> GetCachedRecommendationsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores the calculated recommendations for a user in the cache with an optional TTL.
    /// </summary>
    /// <param name="userId">Unique identifier of the user.</param>
    /// <param name="recommendations">List of ranked recommendation items to cache.</param>
    /// <param name="expiry">Optional expiration timespan; defaults to configured TTL if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetCachedRecommendationsAsync(
        string userId,
        IReadOnlyList<RankedResult> recommendations,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates and removes the cached recommendations for a user (e.g. after a new interaction).
    /// </summary>
    /// <param name="userId">Unique identifier of the user whose cache should be invalidated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InvalidateUserCacheAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the real-time popularity score of a catalog item in the sorted set.
    /// </summary>
    /// <param name="itemId">Unique identifier of the item.</param>
    /// <param name="score">Score delta to increment by (default 1.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new popularity score after increment.</returns>
    Task<double> IncrementItemPopularityAsync(
        string itemId,
        double score = 1.0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current real-time popularity score for a catalog item.
    /// </summary>
    /// <param name="itemId">Unique identifier of the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Current popularity score, or 0 if item not tracked.</returns>
    Task<long> GetItemPopularityAsync(
        string itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the top most popular item identifiers ranked by popularity score for cold-start scenarios.
    /// </summary>
    /// <param name="count">Maximum number of popular item IDs to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of top popular item IDs.</returns>
    Task<IReadOnlyList<string>> GetTopPopularItemIdsAsync(
        int count,
        CancellationToken cancellationToken = default);
}
