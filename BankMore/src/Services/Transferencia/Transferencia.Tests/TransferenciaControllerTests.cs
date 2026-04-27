using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Transferencia.API.Controllers;
using Transferencia.Application.Commands;

namespace Transferencia.Tests;

public class TransferenciaControllerTests
{
    [Fact]
    public async Task Transferir_DeveChamarMediatorSendERetornarNoContent()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<TransferirCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var controller = new TransferenciaController(mediatorMock.Object);
        var command = new TransferirCommand
        {
            RequisicaoId = Guid.NewGuid().ToString(),
            Data = DateTime.Now.ToString("yyyy-MM-dd"),
            NumeroContaDestino = 1,
            NumeroContaOrigem = 2,
            Valor = 100.00m
        };

        // Act
        var result = await controller.Transferir(command);

        // Assert
        mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }
}
