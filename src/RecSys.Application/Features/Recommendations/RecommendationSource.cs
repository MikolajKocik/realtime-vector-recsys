namespace RecSys.Application.Features.Recommendations;

public enum RecommendationSource
{
    RedisCache = 1,
    PgVectorHnsw = 2,
    ColdStart = 3
}
