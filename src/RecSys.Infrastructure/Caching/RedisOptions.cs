namespace RecSys.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = "localhost:6379";
    public string PopularityKey { get; init; } = "items:popularity";
    public string UserCachePrefix { get; init; } = "recs:user:";
    public TimeSpan DefaultCacheTtl { get; init; } = TimeSpan.FromMinutes(10);
}
