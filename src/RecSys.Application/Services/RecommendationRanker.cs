using RecSys.Application.Abstractions;
using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.Services;

/// <summary>
/// Re-ranks candidates by combining cosine similarity, logarithmic popularity, and category diversity penalty.
/// </summary>
public sealed class RecommendationRanker : IRecommendationRanker
{
    public IReadOnlyList<RankedResult> Rank(
        IEnumerable<CandidateItem> candidates,
        int topK = 5,
        float similarityWeight = 0.7F,
        float popularityWeight = 0.3F,
        float categoryPenaltyStep = 0.15F
    )
    {
        var candidateList = candidates.ToList();
        if (candidateList.Count == 0)
        {
            return [];
        }

        // normalization process from higher popularity among candidates
        long maxPopularity = candidateList.Max(c => c.PopularityScore);
        float logMaxPopularity = maxPopularity > 0
            ? MathF.Log(1f + maxPopularity)
            : 1f;

        var rankedResults = new List<RankedResult>(capacity: topK);
        var categoryCounts = new Dictionary<int, int>();

        while (rankedResults.Count < topK && candidateList.Count > 0)
        {
            CandidateItem? bestCandidate = null;
            RankedResult? bestResult = null;
            int bestIndex = -1;

            for (int i = 0; i < candidateList.Count; i++)
            {
                var candidate = candidateList[i];

                float normalizedPop = maxPopularity > 0
                    ? MathF.Log(1f + candidate.PopularityScore) / logMaxPopularity
                    : 0f;

                // check how many items got from this category and gave a penalty
                categoryCounts.TryGetValue(candidate.Category, out int selectedCategoryCount);
                float diversityPenalty = selectedCategoryCount * categoryPenaltyStep;

                // final result with weights and greedy ranking
                float finalScore = (similarityWeight * candidate.CosineSimilarity)
                    + (popularityWeight * normalizedPop)
                    - diversityPenalty;

                if (bestResult is null || finalScore > bestResult.FinalScore)
                {
                    bestCandidate = candidate;
                    bestIndex = i;
                    bestResult = new RankedResult(
                        candidate.Id,
                        candidate.Title,
                        candidate.Category,
                        finalScore,
                        candidate.CosineSimilarity,
                        normalizedPop,
                        diversityPenalty
                    );
                }
            }

            if (bestCandidate is null || bestResult is null)
            {
                break;
            }

            rankedResults.Add(bestResult);

            // increase the counter due to keep next category harder to choose
            categoryCounts[bestCandidate.Category] =
                categoryCounts.GetValueOrDefault(bestCandidate.Category) + 1;

            candidateList.RemoveAt(bestIndex);
        }

        return rankedResults;
    }
}
