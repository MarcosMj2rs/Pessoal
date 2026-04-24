using ContaCorrente.Domain.Entities;

namespace ContaCorrente.Application.Interfaces
{
    public interface IMovimentoRepository
    {
        Task InserirAsync(Movimento movimento);

        Task<decimal> ObterSaldoAsync(string contaId);
    }
}
