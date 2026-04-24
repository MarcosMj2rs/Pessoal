namespace Transferencia.Application.Interfaces
{
    public interface IContaCorrenteClient
    {
        Task DebitarAsync(string requisicaoId, decimal valor, string token);

        Task CreditarAsync(string requisicaoId, long numeroContaDestino, decimal valor, string token);
    }
}