using ContaCorrente.Application.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public IdempotenciaRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> TentarRegistrarAsync(string chave, string requisicao, string tipoMovimento)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"INSERT INTO idempotencia (chave_idempotencia,
                                              tipo_movimento,
                                              requisicao,
                                              data,
                                              resultado
                                             )
                    VALUES (@Chave,
                            @TipoMovimento,
                            @Requisicao,
                            @Data,
                            @Resultado)";

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                Chave = chave.ToString(),
                TipoMovimento = tipoMovimento,
                Requisicao = requisicao,
                Data = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Resultado ="TRANSITORIO" 
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