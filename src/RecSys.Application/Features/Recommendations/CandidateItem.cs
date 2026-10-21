namespace RecSys.Application.Features.Recommendations;

public sealed record CandidateItem(
    Guid Id,
    string Title,
    int Category,
    float CosineSimilarity,
    long PopularityScore
);