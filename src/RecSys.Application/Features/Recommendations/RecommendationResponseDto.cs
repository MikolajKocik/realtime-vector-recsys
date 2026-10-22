namespace RecSys.Application.Features.Recommendations;

public sealed record RecommendationResponseDto(
    string UserId,
    IReadOnlyList<RankedResult> Items,
    RecommendationSource Source,
    double ExecutionTimeMs
);