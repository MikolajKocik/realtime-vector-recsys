using System.Data;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Provides factory method for creating PostgreSQL database connections.
/// </summary>
public interface INpgSqlProvider
{
    /// <summary>
    /// Creates and returns a new database connection instance for executing queries.
    /// </summary>
    /// <returns>An openable <see cref="IDbConnection"/> instance.</returns>
    IDbConnection CreateConnection();
}
