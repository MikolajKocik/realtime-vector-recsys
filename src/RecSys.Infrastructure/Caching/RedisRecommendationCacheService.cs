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
    private readonly ILogger<RedisRecommendationCacheService> _logger = logger;
    private readonly IOptions<RedisOptions> _options = options;
    private readonly IConnectionMultiplexer _redis = redis;
    private readonly IDatabase _db = redis.GetDatabase();

    private readonly string _popularityKey = options.Value.PopularityKey;
    private readonly string _userCachePrefix = options.Value.UserCachePrefix;
    private readonly TimeSpan _defaultTtl = options.Value.DefaultCacheTtl;


    private static bool IsEmpty(Guid id) => id == Guid.Empty;

    public async Task<IReadOnlyList<RankedResult>?> GetCachedRecommendationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsEmpty(userId))
        {
            _logger.LogEmptyIdentifier();
            return null;
        }

        string key = $"{_userCachePrefix}{userId}";

        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            _logger.LogCacheMiss(userId);
            return null;
        }

        try
        {
            var result = JsonSerializer.Deserialize<IReadOnlyList<RankedResult>>(value.ToString());
            _logger.LogCacheHit(userId, result?.Count ?? 0);
            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogDeserializationFailed(userId, ex);
            return null;
        }
    }

    public async Task<long> GetItemPopularityAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsEmpty(itemId))
        {
            _logger.LogEmptyIdentifier();
            return 0;
        }

        double? score = await _db.SortedSetScoreAsync(_popularityKey, itemId.ToString());
        return (long)(score ?? 0);
    }

    public async Task<IReadOnlyList<Guid>> GetTopPopularItemIdsAsync(int count, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var members = await _db.SortedSetRangeByRankAsync(
                _popularityKey,
                start: 0,
                stop: count - 1,
                order: Order.Descending
            );

            var membersToGuid = members
                .Select(m => Guid.TryParse(m.ToString(), out var id) ? id : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            return membersToGuid.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogPopularityRetrievalFailed(ex);
            return [];
        }
    }

    public async Task<double> IncrementItemPopularityAsync(Guid itemId, double score = 1, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsEmpty(itemId))
        {
            _logger.LogEmptyIdentifier();
            return 0;
        }

        double result = await _db.SortedSetIncrementAsync(_popularityKey, itemId.ToString(), score);
        _logger.LogPopularityIncremented(itemId, score, result);
        return result;
    }

    public async Task InvalidateUserCacheAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsEmpty(userId))
        {
            _logger.LogEmptyIdentifier();
            return;
        }

        string key = $"{_userCachePrefix}{userId}";

        await _db.KeyDeleteAsync(key);
        _logger.LogCacheInvalidated(userId);
    }

    public async Task SetCachedRecommendationsAsync(Guid userId, IReadOnlyList<RankedResult> recommendations, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsEmpty(userId))
        {
            _logger.LogEmptyIdentifier();
            return;
        }

        if (recommendations is null)
        {
            _logger.LogNullRecommendations(userId);
            return;
        }

        try
        {
            string json = JsonSerializer.Serialize(recommendations);
            TimeSpan ttl = expiry ?? _defaultTtl;

            string key = $"{_userCachePrefix}{userId}";
            await _db.StringSetAsync(key, json, ttl);
            _logger.LogRecommendationsCached(recommendations.Count, userId, ttl);
        }
        catch (JsonException ex)
        {
            _logger.LogSerializationFailed(userId, ex);
        }
    }
}
