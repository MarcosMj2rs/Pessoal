using MediatR;

namespace ContaCorrente.Application.Commands
{
    public class DebitarTarifaCommand : IRequest<Unit>
    {
        public string RequisicaoId { get; set; } = string.Empty;

        public string IdContaCorrente { get; set; } = string.Empty;

        public decimal Valor { get; set; }
    }
}