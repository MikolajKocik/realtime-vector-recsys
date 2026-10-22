using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Re-ranks raw retrieval candidates using business logic, popularity, and diversity constraints.
/// </summary>
public interface IRecommendationRanker
{
    /// <summary>
    /// Re-ranks candidates by combining cosine similarity, logarithmic popularity, and category diversity penalty.
    /// </summary>
    /// <param name="candidates">Raw candidate items retrieved from the first retrieval stage (e.g. HNSW).</param>
    /// <param name="topK">The number of items to return in the final recommendation list.</param>
    /// <param name="similarityWeight">Weight assigned to the user taste vector similarity (0.0 to 1.0).</param>
    /// <param name="popularityWeight">Weight assigned to normalized item popularity (0.0 to 1.0).</param>
    /// <param name="categoryPenaltyStep">Penalty subtracted for each already selected item of the same category.</param>
    /// <returns>A top-K sorted list of ranked recommendation results.</returns>
    IReadOnlyList<RankedResult> Rank(
        IEnumerable<CandidateItem> candidates,
        int topK = 5,
        float similarityWeight = 0.7f,
        float popularityWeight = 0.3f,
        float categoryPenaltyStep = 0.15f);
}
