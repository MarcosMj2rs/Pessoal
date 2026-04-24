using System.Data;

namespace ContaCorrente.Application.Common.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}