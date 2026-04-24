using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Exceptions;
using ContaCorrente.Application.Interfaces;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContaCorrente.Application.Handlers
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsultarSaldoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            var contaIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("contaId")?.Value;

            if (string.IsNullOrEmpty(contaIdStr))
                throw new UnauthorizedAccessException();

            var conta = await _contaRepository.ObterPorIdAsync(contaIdStr);

            if (conta is null)
                throw new BusinessException("INVALID_ACCOUNT", "Conta corrente não encontrada.");

            if (!conta.Ativo)
                throw new BusinessException("INACTIVE_ACCOUNT", "Conta corrente está inativa.");

            var saldo = await _movimentoRepository.ObterSaldoAsync(conta.Id);

            return new SaldoResponse
            {
                NumeroConta = conta.Numero,
                Nome = conta.Nome,
                DataHora = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
}