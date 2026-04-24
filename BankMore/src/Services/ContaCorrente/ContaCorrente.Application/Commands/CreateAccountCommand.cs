using MediatR;
using ContaCorrente.Application.DTOs;

namespace ContaCorrente.Application.Commands;

public class CreateAccountCommand : IRequest<CreateAccountResponse>
{
    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}