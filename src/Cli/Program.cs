using System.Text;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

string ext = Path.GetExtension(path).ToLowerInvariant();
string name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

if (name == "mixed")
{
    MixedImportResult mixed = MixedCsvImporter.Load(path);

    int total = mixed.Products.Count + mixed.Customers.Count + mixed.Errors.Count;
    double pct = total == 0 ? 0 : mixed.Errors.Count * 100.0 / total;
    Console.WriteLine($"Статистика: усього={total} товарів={mixed.Products.Count} клієнтів={mixed.Customers.Count} помилок={pct:F1}%");
    Console.WriteLine();

    Console.WriteLine($"Товари ({mixed.Products.Count}):");
    foreach (ProductDto p in mixed.Products)
        Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,8:F2} грн");

    Console.WriteLine($"Клієнти ({mixed.Customers.Count}):");
    foreach (CustomerDto c in mixed.Customers)
        Console.WriteLine($"  {c.Id,-6} {c.Name,-30} {c.Email}");

    if (mixed.Errors.Count > 0)
    {
        Console.WriteLine($"Пропущено рядків: {mixed.Errors.Count}");
        foreach (string e in mixed.Errors)
            Console.WriteLine($"  ! {e}");
    }
}
else
{
    ImportResult<ProductDto> result = ext switch
    {
        ".json" => ProductJsonImporter.Load(path),
        ".csv"  => ProductCsvImporter.Load(path),
        _       => throw new NotSupportedException($"Непідтримуваний формат: {ext}")
    };

    int total = result.Items.Count + result.Errors.Count;
    double pct = total == 0 ? 0 : result.Errors.Count * 100.0 / total;
    Console.WriteLine($"Статистика: усього={total} прийнято={result.Items.Count} пропущено={result.Errors.Count} помилок={pct:F1}%");
    Console.WriteLine();

    Console.WriteLine($"Завантажено записів: {result.Items.Count}");
    foreach (ProductDto p in result.Items.Take(5))
        Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,8:F2} грн");

    if (result.Errors.Count > 0)
    {
        Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
        foreach (string e in result.Errors)
            Console.WriteLine($"  ! {e}");
    }
}

return 0;
