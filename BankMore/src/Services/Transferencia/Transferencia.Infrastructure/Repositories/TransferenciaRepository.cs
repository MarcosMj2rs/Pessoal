using Dapper;
using Transferencia.Application.Commands;
using Transferencia.Application.Interfaces;
using Transferencia.Infrastructure.Persistence;

namespace Transferencia.Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TransferenciaRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InserirAsync(TransferirCommand command)
        {
            command.Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            using var connection = _connectionFactory.CreateConnection();

            var sql = @"INSERT INTO transferencia
                        (
                            idtransferencia,
                            idcontacorrente_origem,
                            idcontacorrente_destino,
                            datamovimento,
                            valor
                        )
                        VALUES
                        (
                            @RequisicaoId,
                            @NumeroContaDestino,
                            @NumeroContaOrigem,
                            @Data,
                            @Valor
                        )";

            await connection.ExecuteAsync(sql, new
            {
                Id = command.RequisicaoId.ToString(),
                ContaDestino = command.NumeroContaDestino,
                ContaOrigem = command.NumeroContaOrigem,
                Valor = command.Valor,
                Data = command.Data
            });
        }
    }
}