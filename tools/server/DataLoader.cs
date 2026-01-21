using System.Globalization;
using System.Security.Cryptography;
using Skender.Stock.Indicators;

namespace Test.SseServer;

internal static class DataLoader
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    internal static IReadOnlyList<Quote> GetLongest()
    {
        string[] possiblePaths = [
            Path.Combine("_testdata", "quotes", "longest.csv"),
            Path.Combine("bin", "Debug", "net10.0", "_testdata", "quotes", "longest.csv"),
            Path.Combine("..", "..", "tests", "indicators", "_testdata", "quotes", "longest.csv")
        ];

        string? filepath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                filepath = path;
                break;
            }
        }

        if (filepath is null)
        {
            throw new FileNotFoundException("Test data file not found: longest.csv");
        }

        return File.ReadAllLines(filepath)
            .Skip(1)
            .Select(QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .ToSortedList();
    }

    internal static Quote GenerateRandomQuote(DateTime timestamp, double seed)
    {
        const double volatility = 0.01;
        const double drift = 0.001 * 0.01;

        double open = Price(seed, volatility * volatility, drift);
        double close = Price(open, volatility, drift);

        double ocMax = Math.Max(open, close);
        double high = Price(seed, volatility * 0.5, 0);
        high = high < ocMax ? (2 * ocMax) - high : high;

        double ocMin = Math.Min(open, close);
        double low = Price(seed, volatility * 0.5, 0);
        low = low > ocMin ? (2 * ocMin) - low : low;

        double volume = Price(seed * 1000, volatility * 2, drift: 0);

        return new Quote(
            Timestamp: timestamp,
            Open: (decimal)open,
            High: (decimal)high,
            Low: (decimal)low,
            Close: (decimal)close,
            Volume: (decimal)volume);
    }

    private static double Price(double seed, double volatility, double drift)
    {
        double u1 = 1.0 - NextDouble();
        double u2 = 1.0 - NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return seed * Math.Exp(drift - (volatility * volatility * 0.5) + (volatility * z));
    }

    private static double NextDouble()
        => RandomNumberGenerator.GetInt32(int.MaxValue) / (double)int.MaxValue;
}

    private static Quote QuoteFromCsv(string csvLine)
    {
        string[] values = csvLine.Split(',');

        return new Quote(
            Timestamp: DateTime.Parse(values[0], EnglishCulture),
            Open: decimal.Parse(values[1], EnglishCulture),
            High: decimal.Parse(values[2], EnglishCulture),
            Low: decimal.Parse(values[3], EnglishCulture),
            Close: decimal.Parse(values[4], EnglishCulture),
            Volume: decimal.Parse(values[5], EnglishCulture));
    }
}
