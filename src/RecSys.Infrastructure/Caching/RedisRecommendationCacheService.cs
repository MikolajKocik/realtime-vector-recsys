using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecSys.Application.Abstractions;
using RecSys.Application.Features.Recommendations;
using StackExchange.Redis;

namespace RecSys.Infrastructure.Caching;

public sealed class RedisRecommendationCacheService(
    ILogger<RedisRecommendationCacheService> logger,
    IOptions<RedisOptions> options,
    IConnectionMultiplexer redis
) : IRecommendationCacheService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly string _popularityKey = options.Value.PopularityKey;
    private readonly string _userCachePrefix = options.Value.UserCachePrefix;
    private readonly TimeSpan _defaultTtl = options.Value.DefaultCacheTtl;

    public async Task<IReadOnlyList<RankedResult>?> GetCachedRecommendationsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogEmptyIdentifier();
            return null;
        }

        string key = $"{_userCachePrefix}{userId}";

        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            logger.LogCacheMiss(userId);
            return null;
        }

        try
        {
            var result = JsonSerializer.Deserialize<IReadOnlyList<RankedResult>>(value.ToString());
            logger.LogCacheHit(userId, result?.Count ?? 0);
            return result;
        }
        catch (JsonException ex)
        {
            logger.LogDeserializationFailed(userId, ex);
            return null;
        }
    }

    public async Task<long> GetItemPopularityAsync(
        string itemId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            logger.LogEmptyIdentifier();
            return 0;
        }

        double? score = await _db.SortedSetScoreAsync(_popularityKey, itemId);
        return (long)(score ?? 0);
    }

    public async Task<IReadOnlyList<string>> GetTopPopularItemIdsAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (count <= 0)
        {
            return [];
        }

        try
        {
            var members = await _db.SortedSetRangeByRankAsync(
                _popularityKey,
                start: 0,
                stop: count - 1,
                order: Order.Descending
            );

            var result = new List<string>(members.Length);
            foreach (var m in members)
            {
                if (!m.IsNullOrEmpty)
                {
                    result.Add(m.ToString());
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogPopularityRetrievalFailed(ex);
            return [];
        }
    }

    public async Task<double> IncrementItemPopularityAsync(
        string itemId,
        double score = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            logger.LogEmptyIdentifier();
            return 0;
        }

        double result = await _db.SortedSetIncrementAsync(_popularityKey, itemId, score);
        logger.LogPopularityIncremented(itemId, score, result);
        return result;
    }

    public async Task InvalidateUserCacheAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogEmptyIdentifier();
            return;
        }

        string key = $"{_userCachePrefix}{userId}";

        await _db.KeyDeleteAsync(key);
        logger.LogCacheInvalidated(userId);
    }

    public async Task SetCachedRecommendationsAsync(
        string userId,
        IReadOnlyList<RankedResult> recommendations,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogEmptyIdentifier();
            return;
        }

        if (recommendations is null)
        {
            logger.LogNullRecommendations(userId);
            return;
        }

        try
        {
            string json = JsonSerializer.Serialize(recommendations);
            TimeSpan ttl = expiry ?? _defaultTtl;

            string key = $"{_userCachePrefix}{userId}";
            await _db.StringSetAsync(key, json, ttl);
            logger.LogRecommendationsCached(recommendations.Count, userId, ttl);
        }
        catch (JsonException ex)
        {
            logger.LogSerializationFailed(userId, ex);
        }
    }
}
