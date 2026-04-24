using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands;

public class LoginCommand : IRequest<LoginResponse>
{
    public string? Cpf { get; set; }
    public int? NumeroConta { get; set; }
    public string Senha { get; set; } = string.Empty;
}