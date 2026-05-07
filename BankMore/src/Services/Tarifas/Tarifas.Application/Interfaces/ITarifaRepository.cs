namespace Tarifas.Application.Interfaces
{
    public interface ITarifaRepository
    {
        Task InserirAsync(string idContaCorrente, decimal valor);
    }
}
