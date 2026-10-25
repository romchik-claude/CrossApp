using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public bool IsConfirmed { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal Total => _lines.Sum(l => l.Total);

    private Order(string id, string customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    // Єдиний спосіб створити замовлення: усі перевірки тут.
    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Номер замовлення обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта обов'язковий", nameof(customerId));

        return new Order(id.Trim().ToUpperInvariant(), customerId.Trim());
    }

    public void AddLine(string productId, string name, decimal price, int quantity)
    {
        if (IsConfirmed)
            throw new InvalidOperationException(
                $"Замовлення {Id} вже підтверджене, рядки додавати не можна");

        // Усі перевірки аргументів у OrderLine.Create; стан змінюється лише після них.
        OrderLine line = OrderLine.Create(productId, name, price, quantity);
        _lines.Add(line);
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення {Id} вже підтверджене");
        if (_lines.Count == 0)
            throw new InvalidOperationException(
                $"Не можна підтвердити порожнє замовлення {Id}: додайте хоча б один рядок");

        IsConfirmed = true;
    }

    // Мапінг у формат тижня 3 і назад; знадобиться сховищу тижня 5.
    public OrderDto ToDto() =>
        new(Id, CustomerId, IsConfirmed, _lines.Select(l => l.ToDto()).ToList());

    // Відновлює через Create / AddLine / Confirm, тобто ті самі інваріанти.
    public static Order FromDto(OrderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(dto.Lines);

        Order order = Create(dto.Id, dto.CustomerId);
        foreach (OrderLineDto l in dto.Lines)
        {
            ArgumentNullException.ThrowIfNull(l);
            order.AddLine(l.ProductId, l.Name, l.Price, l.Quantity);
        }
        if (dto.IsConfirmed)
            order.Confirm();

        return order;
    }

    public override string ToString() =>
        $"{Id} (клієнт {CustomerId}), рядків: {_lines.Count}, сума: {Total:F2} грн, " +
        (IsConfirmed ? "підтверджене" : "чернетка");
}
