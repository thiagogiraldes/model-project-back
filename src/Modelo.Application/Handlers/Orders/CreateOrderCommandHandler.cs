using MediatR;
using Microsoft.Extensions.Logging;
using Modelo.Application.Commands.Orders;
using Modelo.Application.Interfaces;
using Modelo.Domain.Entities;
using Modelo.Domain.Events;
using Modelo.Domain.Interfaces;

namespace Modelo.Application.Handlers.Orders;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _publisher;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orders,
        IUnitOfWork uow,
        IEventPublisher publisher,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orders = orders;
        _uow = uow;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = new Order(request.CustomerId, request.TotalAmount);

        await _orders.AddAsync(order, ct);
        await _uow.CommitAsync(ct);

        var evt = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount
        };

        await _publisher.PublishAsync("modelo.orders.order-created.v1", evt, ct);

        _logger.LogInformation("Order criado e evento publicado. OrderId={OrderId}", order.Id);

        return order.Id;
    }
}
