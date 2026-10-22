using RecSys.Application.Features.Recommendations;
using RecSys.Application.Services;

namespace RecSys.Application.Tests.Services;

public class RecommendationRankerTests
{
    private readonly RecommendationRanker _sut = new();

    [Fact]
    public void Rank_ShouldPrioritizeDifferentCategory_WhenDiversityPenaltyIsApplied()
    {
        // Arrange
        const int categoryPython = 1;
        const int categoryDevOps = 2;

        var pythonBest = new CandidateItem(
            Id: Guid.NewGuid().ToString(),
            Title: "Python Basics",
            Category: categoryPython,
            CosineSimilarity: 0.90f,
            PopularityScore: 100
        );

        var pythonSecond = new CandidateItem(
            Id: Guid.NewGuid().ToString(),
            Title: "Python Advanced",
            Category: categoryPython,
            CosineSimilarity: 0.88f,
            PopularityScore: 100
        );

        var devopsItem = new CandidateItem(
            Id: Guid.NewGuid().ToString(),
            Title: "Docker Fundamentals",
            Category: categoryDevOps,
            CosineSimilarity: 0.80f,
            PopularityScore: 100
        );

        var candidates = new[] { pythonBest, pythonSecond, devopsItem };

        // Act: Category penalty is 0.15.
        // Round 1: pythonBest wins with highest score.
        // Round 2: pythonSecond gets a penalty of 0.15 (0.88 - 0.15 = 0.73),
        // allowing devopsItem (0.80) to jump ahead to 2nd place!
        var results = _sut.Rank(
            candidates,
            topK: 3,
            similarityWeight: 1.0f,
            popularityWeight: 0.0f,
            categoryPenaltyStep: 0.15f
        );

        // Assert
        results.Should().HaveCount(3);
        results[0].Title.Should().Be("Python Basics");
        results[1].Title.Should().Be("Docker Fundamentals");
        results[2].Title.Should().Be("Python Advanced");
    }

    [Fact]
    public void Rank_ShouldReturnExactTopK_WhenMoreCandidatesProvided()
    {
        // Arrange
        var candidates = Enumerable.Range(1, 10)
            .Select(i => new CandidateItem(
                Id: Guid.NewGuid().ToString(),
                Title: $"Item {i}",
                Category: i,
                CosineSimilarity: 0.5f + (i * 0.04f),
                PopularityScore: i * 10
            ))
            .ToList();

        // Act
        var results = _sut.Rank(candidates, topK: 3);

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public void Rank_ShouldReturnEmptyList_WhenCandidatesAreEmpty()
    {
        // Act
        var results = _sut.Rank([]);

        // Assert
        results.Should().BeEmpty();
    }
}
