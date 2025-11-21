namespace Modelo.Domain.Events;

public sealed class OrderCreatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
    public string SchemaVersion { get; init; } = "v1";
    public string Source { get; init; } = "modelo.api";

    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "BRL";
}
