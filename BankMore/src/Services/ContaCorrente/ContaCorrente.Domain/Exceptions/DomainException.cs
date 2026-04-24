namespace ContaCorrente.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Code { get; }

        public DomainException(string code) : base(code)
            => Code = code;
    }
}