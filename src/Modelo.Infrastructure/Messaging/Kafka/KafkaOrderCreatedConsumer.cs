using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modelo.Domain.Events;

namespace Modelo.Infrastructure.Messaging.Kafka;

public sealed class KafkaOrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<KafkaOrderCreatedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    private const string TopicName = "modelo.orders.order-created.v1";

    public KafkaOrderCreatedConsumer(
        ILogger<KafkaOrderCreatedConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
                GroupId = _configuration["Kafka:GroupId"] ?? "modelo-orders-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(TopicName);

            _logger.LogInformation("KafkaOrderCreatedConsumer iniciado. Topic={Topic}", TopicName);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);
                        if (result?.Message is null) continue;

                        var json = result.Message.Value;
                        var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                        _logger.LogInformation("OrderCreatedEvent recebido via Kafka: {@Event}", evt);

                        using var scope = _scopeFactory.CreateScope();
                        // TODO: chamar Application/Domain
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Erro ao consumir mensagem do Kafka");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);
    }
}
