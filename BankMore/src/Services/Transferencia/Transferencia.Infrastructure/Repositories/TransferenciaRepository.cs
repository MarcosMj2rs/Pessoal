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

            var sqlBusca = @"SELECT idcontacorrente FROM contacorrente WHERE numero = @Numero";

            var idContaOrigem = await connection.QueryFirstOrDefaultAsync<string>(sqlBusca, new { Numero = command.NumeroContaOrigem });
            var idContaDestino = await connection.QueryFirstOrDefaultAsync<string>(sqlBusca, new { Numero = command.NumeroContaDestino });

            if (idContaOrigem is null || idContaDestino is null)
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
                IdContaOrigem = idContaOrigem,
                IdContaDestino = idContaDestino,
                Data = command.Data,
                Valor = command.Valor
            });
        }
    }
}