using DomainEntities = ContaCorrente.Domain.Entities;

namespace ContaCorrente.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task CriarAsync(DomainEntities.ContaCorrente conta);

        Task<int> ObterProximoNumeroContaAsync();

        Task<DomainEntities.ContaCorrente?> ObterPorNumeroAsync(int? numero);

        Task<DomainEntities.ContaCorrente?> ObterPorCpfAsync(string cpf);

        Task<DomainEntities.ContaCorrente?> ObterPorIdAsync(string id);

        Task AtualizarAsync(DomainEntities.ContaCorrente conta);
    }
}
