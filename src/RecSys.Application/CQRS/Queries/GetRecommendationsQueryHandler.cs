using System.Diagnostics;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RecSys.Application.Abstractions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.Features.Recommendations;
using RecSys.Application.Logging;

namespace RecSys.Application.CQRS.Queries;

public sealed class GetRecommendationsQueryHandler(
    IRecommendationCacheService cacheService,
    INpgSqlProvider npgSqlProvider,
    IRecommendationRanker ranker,
    ILogger<GetRecommendationsQueryHandler> logger
) : IRequestHandler<GetRecommendationsQuery, Result<RecommendationResponseDto>>
{
    public async Task<Result<RecommendationResponseDto>> Handle(
        GetRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        long startTimestamp = Stopwatch.GetTimestamp();

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            logger.LogEmptyQueryUserId();
            return new ErrorMessage(
                "Recommendations.InvalidUserId",
                ErrorType.BadRequest,
                "User identifier cannot be empty.");
        }

        if (request.TopK <= 0)
        {
            logger.LogInvalidTopK(request.TopK);
            return new ErrorMessage(
                "Recommendations.InvalidTopK",
                ErrorType.BadRequest,
                "TopK must be greater than zero.");
        }

        try
        {
            var cached = await cacheService.GetCachedRecommendationsAsync(
                request.UserId,
                cancellationToken
            );

            if (cached is not null)
            {
                logger.LogServingFromCache(request.UserId, cached.Count);
                double elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

                return new RecommendationResponseDto(
                    request.UserId,
                    cached,
                    RecommendationSource.RedisCache,
                    elapsedMs
                );
            }

            using var connection = npgSqlProvider.CreateConnection();

            const string profileSql = """
                SELECT "UserId", "TasteVector"::text AS "TasteVectorText"
                FROM recsys."UserProfiles"
                WHERE "UserId" = @UserId;
            """;

            var userProfile = await connection.QuerySingleOrDefaultAsync<UserTasteProfileRow>(
                new CommandDefinition(
                    profileSql,
                    new { request.UserId },
                    cancellationToken: cancellationToken
                )
            );

            if (userProfile is null)
            {
                var topItemIds = await cacheService.GetTopPopularItemIdsAsync(
                    request.TopK,
                    cancellationToken
                );

                List<CandidateItem> coldCandidates = [];

                if (topItemIds.Count > 0)
                {
                    const string sqlByIds = """
                        SELECT "Id", "Title", "Category", 0.0::real AS "CosineSimilarity", "PopularityScore"
                        FROM recsys."CatalogItems"
                        WHERE "Id" = ANY(@Ids);
                    """;

                    IEnumerable<CandidateItem> itemsFromRedisIds = await connection.QueryAsync<CandidateItem>(
                        new CommandDefinition(
                            sqlByIds,
                            new { Ids = topItemIds.ToArray() },
                            cancellationToken: cancellationToken
                        )
                    );

                    coldCandidates.AddRange(itemsFromRedisIds);
                }

                if (coldCandidates.Count == 0)
                {
                    const string sqlTopPopular = """
                        SELECT "Id", "Title", "Category", 0.0::real AS "CosineSimilarity", "PopularityScore"
                        FROM recsys."CatalogItems"
                        ORDER BY "PopularityScore" DESC
                        LIMIT @TopK;
                    """;

                    IEnumerable<CandidateItem> popularFromDb = await connection.QueryAsync<CandidateItem>(
                        new CommandDefinition(
                            sqlTopPopular,
                            new { TopK = request.TopK },
                            cancellationToken: cancellationToken
                        )
                    );

                    coldCandidates.AddRange(popularFromDb);
                }

                var coldResults = coldCandidates.Select(c => new RankedResult(
                    c.Id,
                    c.Title,
                    c.Category,
                    FinalScore: c.PopularityScore,
                    CosineSimilarity: 0f,
                    NormalizedPopularity: 1f,
                    DiversityPenalty: 0f
                )).AsList();

                logger.LogServingColdStart(request.UserId, coldResults.Count);
                double elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

                return new RecommendationResponseDto(
                    request.UserId,
                    coldResults,
                    RecommendationSource.ColdStart,
                    elapsedMs
                );
            }
            else
            {
                const string vectorSql = """
                    SELECT 
                        "Id",
                        "Title",
                        "Category",
                        CAST(1.0 - ("Embedding" <=> @Vector::vector) AS real) AS "CosineSimilarity",
                        "PopularityScore"
                    FROM recsys."CatalogItems"
                    ORDER BY "Embedding" <=> @Vector::vector
                    LIMIT 50;
                """;

                var candidates = (await connection.QueryAsync<CandidateItem>(
                    new CommandDefinition(
                        vectorSql,
                        new { Vector = userProfile.TasteVectorText },
                        cancellationToken: cancellationToken
                    )
                )).ToList();

                var rankedResults = ranker.Rank(candidates, topK: request.TopK);

                await cacheService.SetCachedRecommendationsAsync(
                    request.UserId,
                    rankedResults,
                    cancellationToken: cancellationToken
                );

                logger.LogServingHnsw(request.UserId, candidates.Count, rankedResults.Count);
                double totalElapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

                return new RecommendationResponseDto(
                    request.UserId,
                    rankedResults,
                    RecommendationSource.PgVectorHnsw,
                    totalElapsedMs
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogRecommendationError(ex, request.UserId);
            return new ErrorMessage(
                "Recommendations.QueryFailed",
                ErrorType.Failure,
                "Failed to calculate recommendations.");
        }
    }
}

public sealed record UserTasteProfileRow(string UserId, string TasteVectorText);
