namespace ContaCorrente.Application.DTOs
{
    public class SaldoResponse
    {
        public long NumeroConta { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public decimal Saldo { get; set; }
    }
}