namespace RecSys.Domain.Entities;

/// <summary>
/// Represents a user entity for demonstration and interaction assignment.
/// </summary>
public sealed class User
{
    public string Id { get; private set; }
    public string Email { get; private set; }

    private User() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with a sequential UUID v7 identifier.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <exception cref="ArgumentException">Thrown when the email is empty or whitespace.</exception>
    public User(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        Id = $"u_{Guid.CreateVersion7()}";
        Email = email;
    }
}
