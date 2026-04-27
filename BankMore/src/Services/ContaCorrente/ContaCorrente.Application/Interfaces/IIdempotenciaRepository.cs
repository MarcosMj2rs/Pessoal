namespace ContaCorrente.Application.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<bool> TentarRegistrarAsync(string chave, string requisicao, string tipoMovimento);

        Task AtualizarResultadoAsync(string chave, string resultado);
    }
}