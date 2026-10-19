using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using RecSys.Domain.Enums;

namespace RecSys.Domain.Entities;

/// <summary>
/// Represents a catalog item (product, article, or content) subject to vector-based recommendation.
/// </summary>
public sealed class CatalogItem
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public ItemCategory Category { get; private set; }
    public string Description { get; private set; }

    [Column(TypeName = "vector(3)")]
    public Vector Embedding { get; private set; }
    public int PopularityScore { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private CatalogItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogItem"/> class along with its embedding representation.
    /// </summary>
    /// <param name="title">The title or name of the catalog item.</param>
    /// <param name="category">The business category of the item.</param>
    /// <param name="description">The detailed textual description of the item.</param>
    /// <param name="embedding">The high-dimensional feature embedding vector representing the item.</param>
    /// <exception cref="ArgumentException">Thrown when title or description is empty or whitespace, or embedding is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when embedding is null.</exception>
    public CatalogItem(
        string title,
        ItemCategory category,
        string description,
        float[] embedding
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(embedding);
        if (embedding.Length != 3)
        {
            throw new ArgumentException(
                "Vector must have exactly 3 dimensions.",
                nameof(embedding)
            );
        }

        Embedding = new Vector(embedding);

        Title = title;
        Category = category;
        Description = description;

        Id = $"eci_{Guid.CreateVersion7()}";
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Increments the popularity score of the catalog item by the specified amount.
    /// </summary>
    /// <param name="amount">The positive increment amount (defaults to 1).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the amount is less than or equal to zero.</exception>
    public void IncrementPopularity(int amount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
        PopularityScore += amount;
    }
}
