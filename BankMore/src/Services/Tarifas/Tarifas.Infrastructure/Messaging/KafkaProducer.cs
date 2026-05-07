using Confluent.Kafka;
using Tarifas.Application.Interfaces;

namespace Tarifas.Infrastructure.Messaging
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublicarAsync(string topico, string mensagem)
        {
            await _producer.ProduceAsync(topico, new Message<Null, string>
            {
                Value = mensagem
            });
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
