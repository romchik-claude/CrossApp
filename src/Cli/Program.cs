using System.Text;
using Cli;

Console.OutputEncoding = Encoding.UTF8;

return args switch
{
    [] or ["domain"] => DomainDemo.Run(),     // лаба 4 (за замовчуванням)
    ["env"]          => EnvironmentDemo.Run(), // лаба 2
    [var path]       => ImportDemo.Run(path),  // лаба 3 (шлях до .csv / .json / mixed.csv)
    _                => Usage()
};

static int Usage()
{
    Console.WriteLine("Використання: Cli [domain | env | <файл.csv | файл.json>]");
    return 1;
}
