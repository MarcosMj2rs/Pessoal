using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Interfaces;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Exceptions;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ContaCorrente.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand, Unit>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentarContaHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository,
            IHttpContextAccessor httpContextAccessor,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
            _httpContextAccessor = httpContextAccessor;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<Unit> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            var contaIdStr = ObterContaIdAutenticado();

            var contaLogada = await _contaRepository.ObterPorIdAsync(contaIdStr)
                ?? throw new DomainException("INVALID_ACCOUNT");

            var contaDestino = await ObterContaDestinoAsync(request, contaLogada);

            ValidarTransacao(contaLogada, contaDestino, request);

            var tipo = request.Tipo!.ToUpper();

            var movimento = new Movimento(
                request.RequisicaoId,
                contaDestino.Id,
                request.Valor,
                tipo
            );

            var conseguiuProcessar = await _idempotenciaRepository.TentarRegistrarAsync(
                request.RequisicaoId,
                JsonSerializer.Serialize(request),
                tipo
            );

            if (!conseguiuProcessar)
                return Unit.Value;

            await _movimentoRepository.InserirAsync(movimento);

            await _idempotenciaRepository.AtualizarResultadoAsync(request.RequisicaoId, "OK");

            return Unit.Value;
        }

        #region[[HELPERS]]
        private string ObterContaIdAutenticado()
        {
            var contaId = _httpContextAccessor.HttpContext?.User.FindFirst("contaId")?.Value;

            if (string.IsNullOrEmpty(contaId))
                throw new UnauthorizedAccessException();

            return contaId;
        }

        private async Task<Domain.Entities.ContaCorrente> ObterContaDestinoAsync(
            MovimentarContaCommand request,
            Domain.Entities.ContaCorrente contaLogada)
        {
            var isMesmaConta = !request.NumeroConta.HasValue
                || request.NumeroConta == contaLogada.Numero;

            if (isMesmaConta)
                return contaLogada;

            var numeroAlvo = request.NumeroConta is 0 or null
                           ? contaLogada.Numero
                           : request.NumeroConta;

            return await _contaRepository.ObterPorNumeroAsync(numeroAlvo)
                ?? throw new DomainException("INVALID_ACCOUNT");
        }

        private static void ValidarTransacao(
            Domain.Entities.ContaCorrente contaLogada,
            Domain.Entities.ContaCorrente contaDestino,
            MovimentarContaCommand request)
        {
            if (!contaDestino.Ativo)
                throw new DomainException("INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                throw new DomainException("INVALID_VALUE");

            var tipo = request.Tipo?.ToUpper();

            if (tipo != "C" && tipo != "D")
                throw new DomainException("INVALID_TYPE");

            if (contaDestino.Id != contaLogada.Id && tipo == "D")
                throw new DomainException("INVALID_TYPE");
        }
        #endregion
    }
}