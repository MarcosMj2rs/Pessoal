using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContaCorrenteController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContaCorrenteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch
        {
            return Unauthorized(new
            {
                tipo = "USER_UNAUTHORIZED",
                mensagem = "Usuário ou senha inválidos"
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CriarConta([FromBody] CreateAccountCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                tipo = "INVALID_DOCUMENT",
                mensagem = ex.Message
            });
        }
    }

    [Authorize]
    [HttpPost("inativar")]
    public async Task<IActionResult> InativarConta([FromBody] InativarContaCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpPost("movimentar")]
    public async Task<IActionResult> Movimentar([FromBody] MovimentarContaCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpGet("saldo")]
    public async Task<IActionResult> Saldo()
    {
        var result = await _mediator.Send(new ConsultarSaldoQuery());
        return Ok(result);
    }
}