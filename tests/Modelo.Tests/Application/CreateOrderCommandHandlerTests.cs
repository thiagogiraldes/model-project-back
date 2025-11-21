using FluentAssertions;
using Microsoft.Extensions.Logging;
using Modelo.Application.Commands.Orders;
using Modelo.Application.Handlers.Orders;
using Modelo.Application.Interfaces;
using Modelo.Domain.Entities;
using Modelo.Domain.Interfaces;
using Moq;
using Xunit;

namespace Modelo.Tests.Application;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Should_Create_Order_And_Publish_Event()
    {
        var repository = new Mock<IOrderRepository>();
        var uow = new Mock<IUnitOfWork>();
        var publisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<CreateOrderCommandHandler>>();

        var handler = new CreateOrderCommandHandler(
            repository.Object,
            uow.Object,
            publisher.Object,
            logger.Object);

        var command = new CreateOrderCommand(Guid.NewGuid(), 250);

        var orderId = await handler.Handle(command, CancellationToken.None);

        repository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        publisher.Verify(p => p.PublishAsync(
            "modelo.orders.order-created.v1",
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        orderId.Should().NotBeEmpty();
    }
}
