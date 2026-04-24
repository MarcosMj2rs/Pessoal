using Transferencia.Application.Commands;

namespace Transferencia.Application.Interfaces
{
    public interface ITransferenciaRepository
    {
        Task InserirAsync(TransferirCommand command);
    }
}