using Confluent.Kafka;
using ContaCorrente.Application.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ContaCorrente.Infrastructure.Messaging
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(string bootstrapServers,
                             string topic,
                             IServiceScopeFactory scopeFactory,
                             ILogger<KafkaConsumer> logger)
        {
            _bootstrapServers = bootstrapServers;
            _topic = topic;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "conta-corrente-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            _logger.LogInformation("Kafka consumer iniciado. Aguardando mensagens no tópico {Topico}", _topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var mensagem = JsonSerializer.Deserialize<TarifacaoRealizadaMessage>(result.Message.Value);

                    if (mensagem is null) continue;

                    _logger.LogInformation("Tarifação recebida: IdContaCorrente={IdContaCorrente}, Valor={Valor}",
                                            mensagem.IdContaCorrente, mensagem.Valor);

                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var command = new DebitarTarifaCommand
                    {
                        RequisicaoId = Guid.NewGuid().ToString(),
                        IdContaCorrente = mensagem.IdContaCorrente,
                        Valor = mensagem.Valor
                    };

                    await mediator.Send(command);

                    _logger.LogInformation("Tarifação debitada com sucesso para conta {IdContaCorrente}",
                                            mensagem.IdContaCorrente);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar tarifação Kafka");
                }
            }

            consumer.Close();
        }
    }
}
