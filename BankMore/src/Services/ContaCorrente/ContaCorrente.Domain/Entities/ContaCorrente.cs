namespace ContaCorrente.Domain.Entities
{
    public class ContaCorrente
    {
        #region[[PROPRIEDADES]]
        public string Id { get; private set; }
        
        public int Numero { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        public string SenhaHash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;
        #endregion

        #region [[CONSTRUTORES]]
        public ContaCorrente() { }

        public ContaCorrente(string nome, int numero, string senhaHash, string salt, string cpf)
        {
            Id = Guid.NewGuid().ToString();
            Nome = nome;
            Numero = numero;
            SenhaHash = senhaHash;
            Salt = salt;
            Ativo = true;
            Cpf = cpf;

            Validar();
        }
        #endregion

        #region[[MÉTODOS]]
        //Regras de negócio
        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ArgumentException("Nome é obrigatório");

            if (Numero <= 0)
                throw new ArgumentException("Número da conta inválido");

            if (string.IsNullOrWhiteSpace(SenhaHash))
                throw new ArgumentException("Senha inválida");
        }

        // Comportamentos (não setters públicos)
        public void Inativar()
        {
            if (!Ativo)
                throw new InvalidOperationException("Conta já está inativa");

            Ativo = false;
        }

        public bool ValidarSenha(string senhaHash)
        {
            return SenhaHash == senhaHash;
        }
        #endregion
    }
}
