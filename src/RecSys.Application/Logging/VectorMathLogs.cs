using Microsoft.Extensions.Logging;

namespace RecSys.Application.Logging;

/// <summary>
/// Source-generated high-performance log messages for vector operations.
/// </summary>
internal static partial class VectorMathLogs
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "CalculateTasteShift rejected: user taste vector is empty.")]
    public static partial void LogEmptyUserVector(this ILogger logger);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "CalculateTasteShift rejected: item embedding vector is empty.")]
    public static partial void LogEmptyItemVector(this ILogger logger);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "CalculateTasteShift rejected: dimension mismatch between user ({UserDim}) and item ({ItemDim}).")]
    public static partial void LogDimensionMismatch(this ILogger logger, int userDim, int itemDim);
}
