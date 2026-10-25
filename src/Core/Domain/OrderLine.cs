using Core.Dto;

namespace Core.Domain;

public sealed class OrderLine
{
    public string ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }
    public decimal Total => Price * Quantity;

    private OrderLine(string productId, string name, decimal price, int quantity)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    // Рядок створюється лише через Order.AddLine, тому фабрика internal.
    internal static OrderLine Create(string productId, string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Ідентифікатор товару обов'язковий", nameof(productId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару не може бути порожньою", nameof(name));
        if (price < 0)
            throw new ArgumentException(
                $"Ціна товару {productId} не може бути від'ємною, отримано {price:F2}", nameof(price));
        if (quantity <= 0)
            throw new ArgumentException(
                $"Кількість товару {productId} має бути більшою за нуль, отримано {quantity}", nameof(quantity));

        return new OrderLine(productId.Trim(), name.Trim(), price, quantity);
    }

    public OrderLineDto ToDto() => new(ProductId, Name, Price, Quantity);

    public override string ToString() => $"{ProductId} {Name} x{Quantity} по {Price:F2}";
}
