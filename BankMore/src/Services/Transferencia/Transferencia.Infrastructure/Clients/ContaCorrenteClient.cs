using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Transferencia.Application.Interfaces;

namespace Transferencia.Infrastructure.Clients
{
    public class ContaCorrenteClient : IContaCorrenteClient
    {
        private readonly HttpClient _httpClient;

        public ContaCorrenteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DebitarAsync(string requisicaoId, decimal valor, string token)
        {
            var body = new
            {
                requisicaoId,
                valor,
                tipo = "D"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/contacorrente/movimentar");
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("DEBIT_FAILED");
        }

        public async Task CreditarAsync(string requisicaoId, long numeroContaDestino, decimal valor, string token)
        {
            var body = new
            {
                requisicaoId,
                numeroConta = numeroContaDestino,
                valor,
                tipo = "C"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/contacorrente/movimentar");
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("CREDIT_FAILED");
        }
    }
}