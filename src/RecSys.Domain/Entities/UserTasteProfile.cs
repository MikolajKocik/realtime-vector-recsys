namespace RecSys.Domain.Entities;

/// <summary>
/// Represents the aggregated taste and preference profile of a user in vector space.
/// </summary>
public sealed class UserTasteProfile
{
    public string UserId { get; init; }
    public IReadOnlyList<float> TasteVector { get; private set; }
    public int InteractionCount { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private UserTasteProfile() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserTasteProfile"/> class based on the user's initial interaction.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="initialTasteVector">The initial preference vector (e.g. embedding of the first interacted item).</param>
    /// <exception cref="ArgumentException">Thrown when user ID or initial taste vector is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when initial taste vector is null.</exception>
    public UserTasteProfile(string userId, float[] initialTasteVector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(initialTasteVector);
        if (initialTasteVector.Length == 0)
        {
            throw new ArgumentException(
                "Initial taste vector cannot be empty.",
                nameof(initialTasteVector)
            );
        }

        UserId = userId;
        TasteVector = initialTasteVector;
        InteractionCount = 1; // first profil interaction
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the user's taste profile with a newly computed vector (taste drift).
    /// Increments the total interaction count and updates the timestamp.
    /// </summary>
    /// <param name="newTaste">The newly shifted and normalized preference vector.</param>
    /// <exception cref="ArgumentException">Thrown when the vector is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the vector is null.</exception>
    public void UpdateTaste(float[] newTaste)
    {
        ArgumentNullException.ThrowIfNull(newTaste);
        if (newTaste.Length == 0)
        {
            throw new ArgumentException(
                "User taste vector cannot be empty.",
                nameof(newTaste)
            );
        }
        TasteVector = newTaste;
        InteractionCount++;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
