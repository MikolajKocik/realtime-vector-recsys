namespace RecSys.Application.Abstractions.Output;

public record ErrorMessage(string Id, ErrorType Type, string Description);
