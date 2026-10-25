using Core.Dto;
using Core.Import;

namespace Cli;

internal static class ImportDemo
{
    public static int Run(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
            return 1;
        }

        string ext = Path.GetExtension(path).ToLowerInvariant();
        string name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

        if (name == "mixed")
        {
            PrintMixed(MixedCsvImporter.Load(path));
            return 0;
        }

        ImportResult<ProductDto>? result = ext switch
        {
            ".json" => ProductJsonImporter.Load(path),
            ".csv"  => ProductCsvImporter.Load(path),
            _       => null
        };

        if (result is null)
        {
            Console.WriteLine($"Непідтримуваний формат: {ext}");
            return 1;
        }

        PrintProducts(result);
        return 0;
    }

    private static void PrintProducts(ImportResult<ProductDto> result)
    {
        PrintStats(result.Items.Count, result.Errors.Count);
        Console.WriteLine();
        Console.WriteLine($"Завантажено записів: {result.Items.Count}");
        foreach (ProductDto p in result.Items.Take(5))
            Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,8:F2} грн");
        PrintErrors(result.Errors);
    }

    private static void PrintMixed(MixedImportResult mixed)
    {
        PrintStats(mixed.Products.Count + mixed.Customers.Count, mixed.Errors.Count);
        Console.WriteLine();
        Console.WriteLine($"Товари ({mixed.Products.Count}):");
        foreach (ProductDto p in mixed.Products)
            Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,8:F2} грн");
        Console.WriteLine($"Клієнти ({mixed.Customers.Count}):");
        foreach (CustomerDto c in mixed.Customers)
            Console.WriteLine($"  {c.Id,-6} {c.Name,-30} {c.Email}");
        PrintErrors(mixed.Errors);
    }

    private static void PrintStats(int accepted, int skipped)
    {
        int total = accepted + skipped;
        double pct = total == 0 ? 0 : skipped * 100.0 / total;
        Console.WriteLine($"Статистика: усього={total} прийнято={accepted} пропущено={skipped} помилок={pct:F1}%");
    }

    private static void PrintErrors(IReadOnlyList<string> errors)
    {
        if (errors.Count == 0)
            return;
        Console.WriteLine($"Пропущено рядків: {errors.Count}");
        foreach (string e in errors)
            Console.WriteLine($"  ! {e}");
    }
}
