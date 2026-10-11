using System.Globalization;
using Core.Dto;

namespace Core.Import;

public sealed record MixedImportResult(
    IReadOnlyList<ProductDto> Products,
    IReadOnlyList<CustomerDto> Customers,
    IReadOnlyList<string> Errors);

public static class MixedCsvImporter
{
    private const char Separator = ';';

    public static MixedImportResult Load(string path)
    {
        var products = new List<ProductDto>();
        var customers = new List<CustomerDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            switch (ParseLine(line))
            {
                case ParseProduct ok:
                    products.Add(ok.Value);
                    break;
                case ParseCustomer ok:
                    customers.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new MixedImportResult(products, customers, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            ["P", var id, var name, var price] when
                decimal.TryParse(price, NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal p) && p >= 0
                => new ParseProduct(new ProductDto(id, name, p)),

            ["P", _, _, var price]
                => new ParseFailed($"ціна '{price}' не є невід'ємним числом"),

            ["P", ..]
                => new ParseFailed($"очікую P;id;name;price, отримав {parts.Length} колонок"),

            ["C", var id, var name, var email] when !string.IsNullOrWhiteSpace(email)
                => new ParseCustomer(new CustomerDto(id, name, email)),

            ["C", ..]
                => new ParseFailed($"очікую C;id;name;email, отримав {parts.Length} колонок"),

            _   => new ParseFailed($"невідомий префікс '{parts[0]}'")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseProduct(ProductDto Value) : ParseOutcome;
    private sealed record ParseCustomer(CustomerDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}
