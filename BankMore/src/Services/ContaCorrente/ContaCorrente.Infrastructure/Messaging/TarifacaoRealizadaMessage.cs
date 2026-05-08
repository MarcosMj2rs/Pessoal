namespace ContaCorrente.Infrastructure.Messaging
{
    public class TarifacaoRealizadaMessage
    {
        public string IdContaCorrente { get; set; } = string.Empty;

        public decimal Valor { get; set; }
    }
}
