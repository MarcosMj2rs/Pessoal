namespace Tarifas.Application.Models
{
    public class TarifacaoRealizadaMessage
    {
        public string IdContaCorrente { get; set; } = string.Empty;

        public decimal Valor { get; set; }
    }
}
