using ContaCorrente.Application.Commands;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Interfaces;
using ContaCorrente.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IJwtService _jwtService;

        public LoginHandler(IContaCorrenteRepository repository, IJwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var hasNumero = request.NumeroConta.HasValue && request.NumeroConta > 0;
            var hasCpf = !string.IsNullOrWhiteSpace(request.Cpf);

            if (!hasNumero && !hasCpf)
                throw new UnauthorizedAccessException("Informe CPF ou número da conta");

            ContaCorrente.Domain.Entities.ContaCorrente? conta = hasNumero
                ? await _repository.ObterPorNumeroAsync(request.NumeroConta!.Value)
                : await _repository.ObterPorCpfAsync(request.Cpf!);

            if (conta is null)
                throw new UnauthorizedAccessException("Usuário não encontrado");

            var senhaHash = GerarHash(request.Senha, conta.Salt);

            if (!conta.ValidarSenha(senhaHash))
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");

            var token = _jwtService.GerarToken(conta.Id);

            return new LoginResponse
            {
                Token = token
            };
        }

        private string GerarHash(string senha, string salt)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(senha + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
