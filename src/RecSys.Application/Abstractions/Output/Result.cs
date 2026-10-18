using RecSys.Application.Abstractions.Output;
namespace RecSys.Application.Abstractions.Output;

public record Result
{
    public bool IsSuccess { get; }
    public ErrorMessage? Error { get; }

    protected Result(bool isSuccess, ErrorMessage? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(ErrorMessage error) => new(false, error
        ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result(ErrorMessage error) => Failure(error);
}

public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(ErrorMessage error) : base(false, error) { }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(ErrorMessage error) => new(error);
}