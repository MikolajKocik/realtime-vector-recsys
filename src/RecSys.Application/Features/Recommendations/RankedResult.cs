namespace RecSys.Application.Features.Recommendations;

public sealed record RankedResult(
    string Id,
    string Title,
    int Category,
    float FinalScore,
    float CosineSimilarity,
    float NormalizedPopularity,
    float DiversityPenalty
);
