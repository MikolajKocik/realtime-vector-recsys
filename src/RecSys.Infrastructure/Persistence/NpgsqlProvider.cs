using System.Data;
using Npgsql;
using RecSys.Application.Abstractions;

namespace RecSys.Infrastructure.Persistence;

public sealed class NpgsqlProvider(string connectionString) : INpgSqlProvider
{
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
