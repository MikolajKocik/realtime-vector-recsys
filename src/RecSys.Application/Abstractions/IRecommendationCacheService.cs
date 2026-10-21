using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Provides caching operations for recommendations and real-time item popularity tracking.
/// </summary>
public interface IRecommendationCacheService
{
    Task<IReadOnlyList<RankedResult>?> GetCachedRecommendationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SetCachedRecommendationsAsync(
        Guid userId,
        IReadOnlyList<RankedResult> recommendations,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    Task InvalidateUserCacheAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<double> IncrementItemPopularityAsync(
        Guid itemId,
        double score = 1.0,
        CancellationToken cancellationToken = default);

    Task<long> GetItemPopularityAsync(
        Guid itemId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetTopPopularItemIdsAsync(
        int count,
        CancellationToken cancellationToken = default);
}
