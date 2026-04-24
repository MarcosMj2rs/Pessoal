namespace ContaCorrente.Application.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<bool> TentarRegistrarAsync(string chave, string requisicao);

        Task AtualizarResultadoAsync(string chave, string resultado);
    }
}