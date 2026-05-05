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

            if (command.NumeroContaOrigem == 0 || command.NumeroContaDestino == 0)
                throw new InvalidOperationException("Conta origem ou destino não encontrada.");

            var sqlInsert = @$"INSERT INTO transferencia
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
                                  @IdContaOrigem,
                                  @IdContaDestino,
                                  @Data,
                                  @Valor
                              );";

            await connection.ExecuteAsync(sqlInsert, new
            {
                RequisicaoId = command.RequisicaoId.ToString(),
                IdContaOrigem = command.NumeroContaOrigem,
                IdContaDestino = command.NumeroContaDestino,
                Data = command.Data,
                Valor = command.Valor
            });
        }
    }
}