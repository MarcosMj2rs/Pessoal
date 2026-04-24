using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Persistence;
using Dapper;
using DomainEntities = ContaCorrente.Domain.Entities;



namespace ContaCorrente.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ContaCorrenteRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CriarAsync(DomainEntities.ContaCorrente conta)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"INSERT INTO contacorrente
                  (idcontacorrente, numero, nome, ativo, senha, salt, cpf)
                  VALUES
                  (@Id, @Numero, @Nome, @Ativo, @SenhaHash, @Salt, @Cpf)";

        await connection.ExecuteAsync(sql, new
        {
            Id = conta.Id.ToString(),
            conta.Numero,
            conta.Nome,
            Ativo = conta.Ativo ? 1 : 0,
            SenhaHash = conta.SenhaHash,
            conta.Salt,
            conta.Cpf
        });
    }

    public async Task<int> ObterProximoNumeroContaAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT IFNULL(MAX(numero), 0) + 1 FROM contacorrente";

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<DomainEntities.ContaCorrente?> ObterPorNumeroAsync(int? numero)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT 
                        idcontacorrente AS Id, 
                        numero AS Numero, 
                        nome AS Nome, 
                        ativo AS Ativo, 
                        senha AS SenhaHash, 
                        salt AS Salt,
                        cpf AS Cpf
                    FROM contacorrente 
                    WHERE numero = @Numero";

        return await connection.QueryFirstOrDefaultAsync<DomainEntities.ContaCorrente>(sql, new { Numero = numero });
    }

    public async Task<DomainEntities.ContaCorrente?> ObterPorCpfAsync(string cpf)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT 
                        idcontacorrente AS Id, 
                        numero AS Numero, 
                        nome AS Nome, 
                        ativo AS Ativo, 
                        senha AS SenhaHash, 
                        salt AS Salt,
                        cpf AS Cpf
                    FROM contacorrente 
                    WHERE cpf = @cpf";

        return await connection.QueryFirstOrDefaultAsync<DomainEntities.ContaCorrente>(sql, new { cpf });
    }

    public async Task AtualizarAsync(DomainEntities.ContaCorrente conta)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"UPDATE contacorrente
                SET ativo = @Ativo
                WHERE idcontacorrente = @Id";

        await connection.ExecuteAsync(sql, new
        {
            Id = conta.Id.ToString(),
            Ativo = conta.Ativo ? 1 : 0
        });
    }

    public async Task<DomainEntities.ContaCorrente?> ObterPorIdAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT 
                        idcontacorrente AS Id, 
                        numero AS Numero, 
                        nome AS Nome, 
                        ativo AS Ativo, 
                        senha AS SenhaHash, 
                        salt AS Salt,
                        cpf AS Cpf
                    FROM contacorrente 
                    WHERE idcontacorrente = @Id";

        return await connection.QueryFirstOrDefaultAsync<DomainEntities.ContaCorrente>(sql, new { Id = id.ToString() });
    }

}