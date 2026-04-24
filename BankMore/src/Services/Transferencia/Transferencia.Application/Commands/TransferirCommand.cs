using MediatR;

namespace Transferencia.Application.Commands
{
    public class TransferirCommand : IRequest<Unit>
    {
        public string RequisicaoId { get; set; }

        public int NumeroContaDestino { get; set; }

        public int NumeroContaOrigem { get; set; }

        public string Data { get; set; }

        public decimal Valor { get; set; }
    }
}