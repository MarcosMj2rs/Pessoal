using MediatR;

namespace ContaCorrente.Application.Commands
{
    public class MovimentarContaCommand : IRequest<Unit>
    {
        public string RequisicaoId { get; set; }
        public int? NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }
}
