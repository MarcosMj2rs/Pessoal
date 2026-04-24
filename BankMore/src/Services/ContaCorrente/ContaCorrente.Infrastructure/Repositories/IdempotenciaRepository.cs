using ContaCorrente.Application.Interfaces;
using ContaCorrente.Infrastructure.Persistence;
using Dapper;
using Microsoft.Data.Sqlite;

public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public IdempotenciaRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> TentarRegistrarAsync(string chave, string requisicao)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"INSERT INTO idempotencia
                    (chave_idempotencia, requisicao, data)
                    VALUES (@Chave, @Requisicao, @Data)";

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                Chave = chave.ToString(),
                Requisicao = requisicao,
                Data = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return true;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return false;
        }
    }

    public async Task AtualizarResultadoAsync(string chave, string resultado)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"UPDATE idempotencia
                    SET resultado = @Resultado
                    WHERE chave_idempotencia = @Chave";

        await connection.ExecuteAsync(sql, new
        {
            Chave = chave.ToString(),
            Resultado = resultado
        });
    }
}