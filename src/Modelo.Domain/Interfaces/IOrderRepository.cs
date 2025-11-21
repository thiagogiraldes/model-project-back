using Modelo.Domain.Entities;

namespace Modelo.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
