using FluentAssertions;
using Modelo.Domain.Entities;
using Xunit;

namespace Modelo.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void Should_Create_Order_With_Valid_Data()
    {
        var customerId = Guid.NewGuid();
        decimal total = 150.75m;

        var order = new Order(customerId, total);

        order.Id.Should().NotBeEmpty();
        order.CustomerId.Should().Be(customerId);
        order.TotalAmount.Should().Be(total);
        order.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
