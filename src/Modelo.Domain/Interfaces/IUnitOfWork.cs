namespace Modelo.Domain.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct = default);
}
