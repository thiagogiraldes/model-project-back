using MediatR;

namespace Modelo.Application.Commands.Orders;

public sealed record CreateOrderCommand(Guid CustomerId, decimal TotalAmount) : IRequest<Guid>;
