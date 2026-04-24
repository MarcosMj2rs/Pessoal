using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Domain.ValueObjects;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace ContaCorrente.Application.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IContaCorrenteRepository _repository;

    public CreateAccountHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar CPF (ValueObject)
        var cpf = new Cpf(request.Cpf);

        // 2. Gerar número da conta (simples por enquanto)
        var numeroConta = await _repository.ObterProximoNumeroContaAsync();

        // 3. Gerar hash + salt
        var salt = Guid.NewGuid().ToString();
        var senhaHash = GerarHash(request.Senha, salt);

        // 4. Criar entidade (incluindo CPF)
        var conta = new Domain.Entities.ContaCorrente(
            request.Nome,
            numeroConta,
            senhaHash,
            salt,
            request.Cpf
        );

        // 5. Persistir (vamos implementar depois via Dapper)
        await _repository.CriarAsync(conta);

        // 6. Retornar resposta
        return new CreateAccountResponse
        {
            NumeroConta = numeroConta
        };
    }

    private string GerarHash(string senha, string salt)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha + salt);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}