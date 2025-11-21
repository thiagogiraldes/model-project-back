namespace Modelo.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken ct = default);
}
