using RecSys.Domain.Enums;

namespace RecSys.Domain.Rules;

/// <summary>
/// Domain policy defining mathematical weights and popularity boosts for user interactions.
/// Maps interaction types to vector learning rates (alpha) and catalog item popularity score deltas.
/// </summary>
public static class InteractionWeightPolicy
{
    /// <summary>
    /// Gets the learning rate (alpha) used to shift the user's taste vector towards the item.
    /// Values typically range between -0.1f and 0.5f.
    /// </summary>
    public static float GetLearningRate(InteractionType type) => type switch
    {
        InteractionType.IgnoredRepeatedly => -0.05f,
        InteractionType.Click => 0.10f,
        InteractionType.Favorite => 0.25f,
        InteractionType.Booked => 0.40f,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported interaction type")
    };

    /// <summary>
    /// Gets the score delta to increase the catalog item's popularity.
    /// </summary>
    public static int GetPopularityBoost(InteractionType type) => type switch
    {
        InteractionType.IgnoredRepeatedly => 0,
        InteractionType.Click => 1,
        InteractionType.Favorite => 5,
        InteractionType.Booked => 20,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported interaction type")
    };
}
