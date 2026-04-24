using MediatR;

namespace ContaCorrente.Application.Commands
{
    public class InativarContaCommand : IRequest<Unit>
    {
        public string Senha { get; set; } = string.Empty;
    }
}
