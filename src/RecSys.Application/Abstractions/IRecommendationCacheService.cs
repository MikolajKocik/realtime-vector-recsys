using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Provides caching operations for recommendations and real-time item popularity tracking.
/// </summary>
public interface IRecommendationCacheService
{
    Task<IReadOnlyList<RankedResult>?> GetCachedRecommendationsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task SetCachedRecommendationsAsync(
        string userId,
        IReadOnlyList<RankedResult> recommendations,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    Task InvalidateUserCacheAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<double> IncrementItemPopularityAsync(
        string itemId,
        double score = 1.0,
        CancellationToken cancellationToken = default);

    Task<long> GetItemPopularityAsync(
        string itemId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetTopPopularItemIdsAsync(
        int count,
        CancellationToken cancellationToken = default);
}
