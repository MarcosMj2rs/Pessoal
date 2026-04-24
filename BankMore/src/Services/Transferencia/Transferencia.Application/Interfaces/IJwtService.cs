namespace Transferencia.Application.Interfaces
{
    public interface IJwtService
    {
        string GerarToken(string contaId);
    }
}
