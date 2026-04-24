using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public class ConsultarSaldoQuery : IRequest<SaldoResponse> { }
}
