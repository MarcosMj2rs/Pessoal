using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tarifas.Application.Interfaces;
using Tarifas.Application.Models;

namespace Tarifas.Infrastructure.Messaging
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topico;
        private readonly decimal _valorTarifa;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(string bootstrapServers,
                             string topico,
                             decimal valorTarifa,
                             IServiceScopeFactory scopeFactory,
                             ILogger<KafkaConsumer> logger)
        {
            _bootstrapServers = bootstrapServers;
            _topico = topico;
            _valorTarifa = valorTarifa;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "tarifas-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topico);

            _logger.LogInformation("Kafka consumer iniciado. Aguardando mensagens no tópico {Topico}", _topico);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var mensagem = JsonSerializer.Deserialize<TransferenciaRealizadaMessage>(result.Message.Value);

                    if (mensagem is null) continue;

                    _logger.LogInformation("Mensagem recebida: RequisicaoId={RequisicaoId}, IdContaCorrente={IdContaCorrente}",
                                            mensagem.RequisicaoId,
                                            mensagem.IdContaCorrente);

                    using var scope = _scopeFactory.CreateScope();

                    var tarifaRepository = scope.ServiceProvider.GetRequiredService<ITarifaRepository>();
                    var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();

                    await tarifaRepository.InserirAsync(mensagem.IdContaCorrente, _valorTarifa);

                    var tarifacaoMessage = JsonSerializer.Serialize(new TarifacaoRealizadaMessage
                    {
                        IdContaCorrente = mensagem.IdContaCorrente,
                        Valor = _valorTarifa
                    });

                    await kafkaProducer.PublicarAsync("tarifacoes-realizadas", tarifacaoMessage);

                    _logger.LogInformation("Tarifação registrada e publicada para conta {IdContaCorrente}, valor {Valor}",
                                            mensagem.IdContaCorrente,
                                            _valorTarifa);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem Kafka");
                }
            }

            consumer.Close();
        }
    }
}
