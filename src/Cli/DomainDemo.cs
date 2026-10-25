using Core.Domain;
using Core.Dto;

namespace Cli;

internal static class DomainDemo
{
    public static int Run()
    {
        Console.WriteLine("=== Сценарій 1: успіх ===");
        Order order = Order.Create("ord-001", "C-001");
        order.AddLine("P-001", "Яблука Голден 1кг", 45.50m, 3);
        order.AddLine("P-002", "Молоко 2.5% 1л", 28.90m, 2);
        Console.WriteLine(order);
        order.Confirm();
        Console.WriteLine(order);

        OrderDto dto = order.ToDto();
        Order restored = Order.FromDto(dto);
        Console.WriteLine($"Відновлено з DTO: {restored}");

        Console.WriteLine();
        Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

        TryDo("рядок до підтвердженого замовлення", () => order.AddLine("P-003", "Хліб", 22m, 1));
        TryDo("повторне підтвердження", () => order.Confirm());
        TryDo("підтвердження порожнього замовлення", () => Order.Create("ord-002", "C-002").Confirm());
        TryDo("кількість нуль", () => Order.Create("ord-003", "C-003").AddLine("P-001", "Яблука", 45.50m, 0));
        TryDo("від'ємна ціна", () => Order.Create("ord-004", "C-004").AddLine("P-001", "Яблука", -10m, 1));
        TryDo("порожній номер замовлення", () => Order.Create(" ", "C-005"));
        TryDo("FromDto з пошкодженими даними", () => Order.FromDto(new OrderDto("ord-005", "C-006", true, [])));

        Console.WriteLine();
        Console.WriteLine($"Після всіх відмов: {order}");
        return 0;
    }

    private static void TryDo(string title, Action action)
    {
        try
        {
            action();
            Console.WriteLine($" {title}: виняток НЕ спрацював, інваріант відсутній!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" {title}: {ex.GetType().Name}, {ex.Message}");
        }
    }
}
