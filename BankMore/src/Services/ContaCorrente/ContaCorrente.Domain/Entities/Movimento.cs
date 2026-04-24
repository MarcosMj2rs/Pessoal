using System;
using System.Collections.Generic;
using System.Text;

namespace ContaCorrente.Domain.Entities
{
    public class Movimento
    {
        public string Id { get; private set; }
        public string ContaId { get; private set; }
        public decimal Valor { get; private set; }
        public string Tipo { get; private set; }
        public DateTime Data { get; private set; }

        public Movimento(string requisicaoId, string contaId, decimal valor, string tipo)
        {
            Id = requisicaoId;
            ContaId = contaId;
            Valor = valor;
            Tipo = tipo;
            Data = DateTime.UtcNow;
        }
    }
}
