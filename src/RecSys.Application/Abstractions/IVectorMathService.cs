using RecSys.Application.Abstractions.Output;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Service providing high-performance vector math operations for recommendation adjustments.
/// </summary>
public interface IVectorMathService
{
    /// <summary>
    /// Calculates the shifted and L2-normalized user taste vector based on item interaction and learning rate.
    /// </summary>
    /// <param name="userVector">Current user taste vector.</param>
    /// <param name="itemVector">Embedding vector of the interacted item.</param>
    /// <param name="learningRate">Interaction learning rate (alpha).</param>
    /// <returns>Result containing the new normalized vector, or error details on validation failure.</returns>
    Result<float[]> CalculateTasteShift(
        ReadOnlySpan<float> userVector,
        ReadOnlySpan<float> itemVector,
        float learningRate);
}
