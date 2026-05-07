namespace Transferencia.Application.Interfaces
{
    public interface IKafkaProducer
    {
        Task PublicarAsync(string topico, string mensagem);
    }
}
