using System.Numerics.Tensors;
using Microsoft.Extensions.Logging;
using RecSys.Application.Abstractions;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.Logging;

namespace RecSys.Application.Services;

public sealed class VectorMathService : IVectorMathService
{
    private readonly ILogger _logger;

    public VectorMathService(ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _logger = loggerFactory.CreateLogger<VectorMathService>();
    }

    public Result<float[]> CalculateTasteShift(
        ReadOnlySpan<float> userVector,
        ReadOnlySpan<float> itemVector,
        float learningRate
        )
    {
        if (userVector.IsEmpty)
        {
            _logger.LogEmptyUserVector();
            return new ErrorMessage(
                "VectorMath.EmptyUserVector",
                ErrorType.BadRequest,
                "User taste vector cannot be empty.");
        }

        if (itemVector.IsEmpty)
        {
            _logger.LogEmptyItemVector();
            return new ErrorMessage(
                "VectorMath.EmptyItemVector",
                ErrorType.BadRequest,
                "Item embedding vector cannot be empty.");
        }

        if (userVector.Length != itemVector.Length)
        {
            _logger.LogDimensionMismatch(userVector.Length, itemVector.Length);

            return new ErrorMessage(
                "VectorMath.DimensionMismatch",
                ErrorType.BadRequest,
                $"Vector dimension mismatch: user has {userVector.Length}, item has {itemVector.Length}.");
        }

        float[] result = new float[userVector.Length];

        // Direction vector from current user taste towards the interacted item
        TensorPrimitives.Subtract(itemVector, userVector, result);

        // Scale displacement by interaction confidence (learning rate)
        TensorPrimitives.Multiply(result, learningRate, result);

        // Apply shift to current user taste
        TensorPrimitives.Add(userVector, result, result);

        // Project back to unit hypersphere for fast cosine dot-product     
        float norm = TensorPrimitives.Norm(result);
        if (norm > 0f)
        {
            TensorPrimitives.Divide(result, norm, result);
        }

        return result;
    }
}
