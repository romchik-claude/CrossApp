// using System.Runtime.InteropServices;

// Console.OutputEncoding = System.Text.Encoding.UTF8;

// Console.WriteLine("CrossApp -- практикум з крос-платформного програмування");
// Console.WriteLine("Студент: Белоусов Роман, група ФЕІ-37");
// Console.WriteLine(new string('=', 55));
// Console.WriteLine($"ОС (OSDescription)   : {RuntimeInformation.OSDescription}");
// Console.WriteLine($"ОС (Environment)     : {Environment.OSVersion}");
// Console.WriteLine($"Архітектура процесу  : {RuntimeInformation.ProcessArchitecture}");
// Console.WriteLine($"Версія .NET (CLR)    : {Environment.Version}");
// Console.WriteLine($"Runtime              : {RuntimeInformation.FrameworkDescription}");
// Console.WriteLine($"Каталог застосунку   : {AppContext.BaseDirectory}");
// Console.WriteLine($"Поточний каталог     : {Environment.CurrentDirectory}");
// Console.WriteLine(new string('=', 55));
// Console.WriteLine("Предметна область: Замовлення (Customer, Product, Order, OrderLine)");

using System.Runtime.InteropServices;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var info = new
{
    OS_Description = RuntimeInformation.OSDescription,
    OS_Environment = Environment.OSVersion.ToString(),
    Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
    DotNet_Version = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    App_Directory = AppContext.BaseDirectory,
    Current_Directory = Environment.CurrentDirectory,
    Subject = "Замовлення (Customer, Product, Order, OrderLine)"
};

if (args.Contains("--json"))
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    Console.WriteLine(JsonSerializer.Serialize(info, options));
}
else
{
    Console.WriteLine("CrossApp -- практикум з крос-платформного програмування");
    Console.WriteLine("Студент: Белоусов Роман, група ФЕІ-37");
    Console.WriteLine(new string('=', 55));
    Console.WriteLine($"ОС (OSDescription)   : {info.OS_Description}");
    Console.WriteLine($"ОС (Environment)     : {info.OS_Environment}");
    Console.WriteLine($"Архітектура процесу  : {info.Architecture}");
    Console.WriteLine($"Версія .NET (CLR)    : {info.DotNet_Version}");
    Console.WriteLine($"Runtime              : {info.Runtime}");
    Console.WriteLine($"Каталог застосунку   : {info.App_Directory}");
    Console.WriteLine($"Поточний каталог     : {info.Current_Directory}");
    Console.WriteLine(new string('=', 55));
    Console.WriteLine($"Предметна область: {info.Subject}");
}