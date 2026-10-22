namespace RecSys.Application.Features.Recommendations;

public sealed record CandidateItem(
    string Id,
    string Title,
    int Category,
    float CosineSimilarity,
    long PopularityScore
);