namespace RecSys.Application.Features.Recommendations;

public sealed record RankedResult(
    Guid Id,
    string Title,
    int Category,
    float FinalScore,
    float cosineSimilarity,
    float NormalizedPopularity,
    float DiversityPenalty
);
