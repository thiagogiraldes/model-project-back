namespace Modelo.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Order() { }

    public Order(Guid customerId, decimal totalAmount)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId inválido", nameof(customerId));

        if (totalAmount <= 0)
            throw new ArgumentException("TotalAmount deve ser maior que zero", nameof(totalAmount));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        TotalAmount = totalAmount;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
