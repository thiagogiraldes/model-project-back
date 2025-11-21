using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modelo.Application.Interfaces;
using Modelo.Domain.Interfaces;
using Modelo.Infrastructure.Messaging.Kafka;
using Modelo.Infrastructure.Messaging.Solace;
using Modelo.Infrastructure.Repositories;

namespace Modelo.Infrastructure.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        services.AddSingleton<KafkaEventPublisher>();
        services.AddSingleton<SolaceEventPublisher>();

        services.AddSingleton<IEventPublisher>(sp =>
        {
            // Padrão inicial Kafka; ajustar para usar Solace se necessário
            return sp.GetRequiredService<KafkaEventPublisher>();
        });

        services.AddHostedService<KafkaOrderCreatedConsumer>();
        services.AddHostedService<SolaceOrderCreatedConsumer>();

        return services;
    }
}
