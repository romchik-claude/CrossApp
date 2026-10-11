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

// Вибір імпортера за розширенням файлу
ImportResult<ProductDto> result = ext switch
{
    ".json" => ProductJsonImporter.Load(path),
    ".csv"  => ProductCsvImporter.Load(path),
    _       => throw new NotSupportedException($"Непідтримуваний формат: {ext}")
};

// Завдання 3: статистика одним рядком
int total  = result.Items.Count + result.Errors.Count;
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

return 0;
