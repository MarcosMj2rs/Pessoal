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
        private readonly IKafkaProducer _kafkaProducer;

        public TransferirHandler(
            IContaCorrenteClient client,
            ITransferenciaRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IKafkaProducer kafkaProducer)
        {
            _client = client;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _kafkaProducer = kafkaProducer;
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
                await _repository.InserirAsync(request);

                var contaId = _httpContextAccessor.HttpContext?.User?.FindFirst("contaId")?.Value;

                var mensagem = JsonSerializer.Serialize(new
                {
                    RequisicaoId = request.RequisicaoId,
                    IdContaCorrente = contaId

                });

                await _kafkaProducer.PublicarAsync("transferencias-realizadas", mensagem);
            }
            catch (Exception)
            {
                await _client.CreditarAsync(request.RequisicaoId, request.NumeroContaDestino, request.Valor, token);

                throw new Exception("TRANSFER_FAILED");
            }

            return Unit.Value;
        }
    }
}