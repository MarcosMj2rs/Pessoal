using ContaCorrente.Application.Commands;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ContaCorrente.Application.Handlers
{
    public class InativarContaHandler : IRequestHandler<InativarContaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InativarContaHandler(IContaCorrenteRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            var contaIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("contaId")?.Value;

            if (string.IsNullOrEmpty(contaIdStr))
                throw new UnauthorizedAccessException();

            var contaId = contaIdStr;

            var conta = await _repository.ObterPorIdAsync(contaId);

            if (conta is null)
                throw new Exception("INVALID_ACCOUNT");

            var senhaHash = GerarHash(request.Senha, conta.Salt);

            if (!conta.ValidarSenha(senhaHash))
                throw new UnauthorizedAccessException("Senha inválida");

            conta.Inativar();

            await _repository.AtualizarAsync(conta);

            return Unit.Value;
        }

        private string GerarHash(string senha, string salt)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
