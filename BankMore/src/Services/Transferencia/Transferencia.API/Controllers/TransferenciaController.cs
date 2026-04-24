using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Application.Commands;

namespace Transferencia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Transferir([FromBody] TransferirCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}