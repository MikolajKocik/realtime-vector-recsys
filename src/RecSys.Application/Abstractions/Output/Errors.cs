namespace RecSys.Application.Abstractions.Output;

public static class Errors
{
    public static ErrorMessage ProfileNotFound { get; } = new(
        $"NotFound-{Guid.CreateVersion7()}", ErrorType.NotFound, "Profile Not found.");
}
