namespace ContaCorrente.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public string Tipo { get; }

        public BusinessException(string tipo, string mensagem) : base(mensagem)
        {
            Tipo = tipo;
        }
    }
}