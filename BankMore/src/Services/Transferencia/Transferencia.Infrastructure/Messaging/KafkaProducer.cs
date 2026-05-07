using Confluent.Kafka;
using Transferencia.Application.Interfaces;

namespace Transferencia.Infrastructure.Messaging
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublicarAsync(string topico, string mensagem)
        {
            try
            {
                var result = await _producer.ProduceAsync(topico, new Message<Null, string>
                {
                    Value = mensagem
                });

                Console.WriteLine($"Mensagem publicada no tópico {topico}: {mensagem}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Erro ao publicar mensagem: {e.Error.Reason}");
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}
