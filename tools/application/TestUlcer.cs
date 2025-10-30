namespace Test.Application;

public static class TestUlcer
{
    public static void Run()
    {
        // Test data
        var quotes = TestData.GetDefault().Take(20).ToList();

        // Series calculation
        var seriesResults = quotes.ToUlcerIndex(14);

        // Hub calculation
        var quoteHub = new QuoteHub();
        quoteHub.Add(quotes);
        var hub = quoteHub.ToUlcerIndexHub(14);
        var hubResults = hub.Results;

        // Compare
        Console.WriteLine($"Series count: {seriesResults.Count}");
        Console.WriteLine($"Hub count: {hubResults.Count}");
        Console.WriteLine("\nFirst few results:");
        for (int i = 0; i < Math.Min(20, seriesResults.Count); i++)
        {
            var s = seriesResults[i];
            var h = hubResults[i];
            Console.WriteLine($"[{i}] Series: {s.UlcerIndex?.ToString("F6") ?? "null",-12} Hub: {h.UlcerIndex?.ToString("F6") ?? "null",-12} Match: {s.UlcerIndex == h.UlcerIndex}");
        }
    }
}
