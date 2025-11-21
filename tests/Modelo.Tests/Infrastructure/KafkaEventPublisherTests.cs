using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Modelo.Infrastructure.Messaging.Kafka;
using Moq;
using Xunit;

namespace Modelo.Tests.Infrastructure;

public class KafkaEventPublisherTests
{
    [Fact]
    public async Task Should_Serialize_Event_And_Send_To_Kafka()
    {
        var producer = new Mock<IProducer<string, string>>();
        producer
            .Setup(p => p.ProduceAsync(
                It.IsAny<string>(),
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResult<string, string>());

        var logger = new Mock<ILogger<KafkaEventPublisher>>();

        var publisher = new KafkaEventPublisher(producer.Object, logger.Object);

        var evt = new { Test = "OK" };

        Func<Task> action = async () =>
            await publisher.PublishAsync("test.topic.v1", evt);

        await action.Should().NotThrowAsync();

        producer.Verify(p =>
            p.ProduceAsync(
                "test.topic.v1",
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
