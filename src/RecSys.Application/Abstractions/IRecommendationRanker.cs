using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Re-ranks raw retrieval candidates using business logic, popularity, and diversity constraints.
/// </summary>
public interface IRecommendationRanker
{
    IReadOnlyList<RankedResult> Rank(
        IEnumerable<CandidateItem> candidates,
        int topK = 5,
        float similarityWeight = 0.7f,
        float popularityWeight = 0.3f,
        float categoryPenaltyStep = 0.15f);
}
