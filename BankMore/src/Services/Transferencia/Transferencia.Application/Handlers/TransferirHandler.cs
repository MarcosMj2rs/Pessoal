using ContaCorrente.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Transferencia.Application.Commands;
using Transferencia.Application.Interfaces;

namespace Transferencia.Application.Handlers
{
    public class TransferirHandler : IRequestHandler<TransferirCommand, Unit>
    {
        private readonly IContaCorrenteClient _client;
        private readonly ITransferenciaRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransferirHandler(
            IContaCorrenteClient client,
            ITransferenciaRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(TransferirCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException();

            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE");

            try
            {
                await _client.DebitarAsync(request.RequisicaoId, request.Valor, token);

                await _client.CreditarAsync(request.RequisicaoId, request.NumeroContaDestino, request.Valor, token);
            }
            catch (Exception)
            {
                await _client.CreditarAsync(request.RequisicaoId, request.NumeroContaDestino, request.Valor, token);

                throw new Exception("TRANSFER_FAILED");
            }

            await _repository.InserirAsync(request);

            return Unit.Value;
        }
    }
}