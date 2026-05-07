using Dapper;
using Tarifas.Application.Interfaces;
using Tarifas.Infrastructure.Persistence;

namespace Tarifas.Infrastructure.Repositories
{
    public class TarifaRepository : ITarifaRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TarifaRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InserirAsync(string idContaCorrente, decimal valor)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"INSERT INTO tarifa 
                        (idtarifa, idcontacorrente, datamovimento, valor)
                        VALUES 
                        (@IdTarifa, @IdContaCorrente, @DataMovimento, @Valor)";

            await connection.ExecuteAsync(sql, new
            {
                IdTarifa = Guid.NewGuid().ToString(),
                IdContaCorrente = idContaCorrente,
                DataMovimento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Valor = valor
            });
        }
    }
}
