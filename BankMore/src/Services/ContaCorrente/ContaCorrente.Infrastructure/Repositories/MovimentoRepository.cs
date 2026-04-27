using ContaCorrente.Application.Interfaces;
using ContaCorrente.Domain.Entities;
using Dapper;

namespace ContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MovimentoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InserirAsync(Movimento mov)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"INSERT INTO movimento
                        (
                            idmovimento,
                            idcontacorrente,
                            valor,
                            tipomovimento,
                            datamovimento
                        )
                        VALUES
                        (
                            @Id, 
                            @ContaId, 
                            @Valor, 
                            @Tipo, 
                            @Data
                        )";

            await connection.ExecuteAsync(sql, new
            {
                Id = mov.Id.ToString(),
                ContaId = mov.ContaId.ToString(),
                mov.Valor,
                mov.Tipo,
                mov.Data
            });
        }

        public async Task<decimal> ObterSaldoAsync(string contaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                        SELECT 
                            COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                            COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0)
                        FROM movimento
                        WHERE idcontacorrente = @ContaId
                    ";

            var saldo = await connection.ExecuteScalarAsync<decimal>(sql, new
            {
                ContaId = contaId.ToString()
            });

            return saldo;
        }
    }
}