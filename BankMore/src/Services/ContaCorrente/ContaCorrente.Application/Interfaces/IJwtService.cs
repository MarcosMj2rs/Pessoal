namespace ContaCorrente.Application.Interfaces
{
    public interface IJwtService
    {
        string GerarToken(string contaId);
    }
}
