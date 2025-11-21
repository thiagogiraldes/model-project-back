using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modelo.Application.Interfaces;

namespace Modelo.Infrastructure.Messaging.Kafka;

public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;

    public KafkaEventPublisher(IConfiguration configuration, ILogger<KafkaEventPublisher> logger)
        : this(BuildProducer(configuration), logger)
    {
    }

    internal KafkaEventPublisher(IProducer<string, string> producer, ILogger<KafkaEventPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    private static IProducer<string, string> BuildProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            Acks = Acks.All
        };

        return new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken ct = default)
    {
        var payload = JsonSerializer.Serialize(@event);
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = payload
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, ct);
            _logger.LogInformation("Mensagem enviada para Kafka. Topic={Topic}, Offset={Offset}", topic, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no Kafka. Topic={Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _producer.Flush();
            _producer.Dispose();
        }
        catch
        {
        }
    }
}
