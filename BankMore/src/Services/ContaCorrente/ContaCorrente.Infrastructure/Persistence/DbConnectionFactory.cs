using ContaCorrente.Application.Common.Interfaces;
using Microsoft.Data.Sqlite;
using System.Data;

namespace ContaCorrente.Infrastructure.Persistence;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}