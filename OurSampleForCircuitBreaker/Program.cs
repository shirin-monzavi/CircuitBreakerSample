
using OurSampleForCircuitBreaker;

IGetOrdersFromEngine repository = new CircuitBreakerForStockMarketEngine();

for (int i = 0; i < 100; i++)
{
    try
    {
        var trades = repository.GetOrders().GetAwaiter().GetResult();
        foreach (var item in trades)
        {
            Console.WriteLine(item);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
    }
    Thread.Sleep(800);
}
Console.WriteLine("Hello, World!");
