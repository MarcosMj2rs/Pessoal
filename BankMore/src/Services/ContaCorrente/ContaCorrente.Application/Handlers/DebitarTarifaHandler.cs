using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Interfaces;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Exceptions;
using ContaCorrente.Domain.Interfaces;
using MediatR;

namespace ContaCorrente.Application.Handlers
{
    public class DebitarTarifaHandler : IRequestHandler<DebitarTarifaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public DebitarTarifaHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<Unit> Handle(DebitarTarifaCommand request, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.IdContaCorrente)
                ?? throw new DomainException("INVALID_ACCOUNT");

            if (!conta.Ativo)
                throw new DomainException("INACTIVE_ACCOUNT");

            var movimento = new Movimento(
                request.RequisicaoId,
                conta.Id,
                request.Valor,
                "D"
            );

            await _movimentoRepository.InserirAsync(movimento);

            return Unit.Value;
        }
    }
}