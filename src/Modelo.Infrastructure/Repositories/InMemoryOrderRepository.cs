using Modelo.Domain.Entities;
using Modelo.Domain.Interfaces;

namespace Modelo.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _storage = new();

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _storage.Add(order);
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = _storage.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(order);
    }
}

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
}
