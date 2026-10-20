using System.Data;

namespace RecSys.Application.Abstractions;

public interface INpgSqlProvider
{
    IDbConnection CreateConnection();
}
