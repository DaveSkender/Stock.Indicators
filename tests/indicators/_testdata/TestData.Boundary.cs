namespace Test.Data;

/// <summary>
/// Generates boundary test data designed to expose floating-point precision issues
/// in bounded indicators. These datasets push calculations to exact mathematical
/// boundaries where precision errors become visible.
/// </summary>
internal static class BoundaryQuotes
{
    /// <summary>
    /// Generates quotes with monotonically increasing closes to push RSI toward 100.
    /// After warmup, all gains with no losses should theoretically produce RSI = 100.
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <param name="startPrice">Starting price value.</param>
    /// <param name="increment">Price increment per bar.</param>
    /// <returns>List of quotes with strictly increasing closes.</returns>
    internal static IReadOnlyList<Quote> GetMonotonicallyIncreasing(
        int bars = 100,
        decimal startPrice = 100m,
        decimal increment = 0.01m)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);

        for (int i = 0; i < bars; i++)
        {
            decimal close = startPrice + (i * increment);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: close - (increment / 2),
                High: close + (increment / 2),
                Low: close - (increment / 2),
                Close: close,
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates quotes with monotonically decreasing closes to push RSI toward 0.
    /// After warmup, all losses with no gains should theoretically produce RSI = 0.
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <param name="startPrice">Starting price value.</param>
    /// <param name="decrement">Price decrement per bar.</param>
    /// <returns>List of quotes with strictly decreasing closes.</returns>
    internal static IReadOnlyList<Quote> GetMonotonicallyDecreasing(
        int bars = 100,
        decimal startPrice = 1000m,
        decimal decrement = 0.01m)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);

        for (int i = 0; i < bars; i++)
        {
            decimal close = startPrice - (i * decrement);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: close + (decrement / 2),
                High: close + (decrement / 2),
                Low: close - (decrement / 2),
                Close: close,
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates quotes where Close = High for Stoch boundary testing.
    /// When Close equals the highest high in the lookback, Stoch should be exactly 100.
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <returns>List of quotes where Close equals High.</returns>
    internal static IReadOnlyList<Quote> GetCloseEqualsHigh(int bars = 100)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);

        for (int i = 0; i < bars; i++)
        {
            decimal price = 100m + (i * 0.1m);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: price - 1m,
                High: price, // Close = High
                Low: price - 2m,
                Close: price, // Close = High
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates quotes where Close = Low for Stoch boundary testing.
    /// When Close equals the lowest low in the lookback, Stoch should be exactly 0.
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <returns>List of quotes where Close equals Low.</returns>
    internal static IReadOnlyList<Quote> GetCloseEqualsLow(int bars = 100)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);

        for (int i = 0; i < bars; i++)
        {
            decimal price = 100m + (i * 0.1m);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: price + 1m,
                High: price + 2m,
                Low: price, // Close = Low
                Close: price, // Close = Low
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates quotes where Close = High = Low (flat candles).
    /// This should produce Stoch = 0 (by convention when highHigh - lowLow = 0).
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <returns>List of quotes with flat candles.</returns>
    internal static IReadOnlyList<Quote> GetFlatCandles(int bars = 100)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);

        for (int i = 0; i < bars; i++)
        {
            decimal price = 100m + (i * 0.01m);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: price,
                High: price,
                Low: price,
                Close: price,
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates alternating up/down quotes for CMO boundary testing.
    /// Alternating gains and losses of equal magnitude should produce CMO = 0.
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <returns>List of quotes with alternating price movements.</returns>
    internal static IReadOnlyList<Quote> GetAlternating(int bars = 100)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);
        decimal basePrice = 100m;

        for (int i = 0; i < bars; i++)
        {
            decimal offset = (i % 2 == 0) ? 0m : 1m;
            decimal close = basePrice + offset;
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: close,
                High: close + 0.5m,
                Low: close - 0.5m,
                Close: close,
                Volume: 1000m));
        }

        return quotes;
    }

    /// <summary>
    /// Generates quotes with extremely small price movements to test precision limits.
    /// Uses values near the limits of double precision (around 1e-14 relative error).
    /// </summary>
    /// <param name="bars">Number of bars to generate.</param>
    /// <returns>List of quotes with tiny price movements.</returns>
    internal static IReadOnlyList<Quote> GetTinyMovements(int bars = 100)
    {
        List<Quote> quotes = new(bars);
        DateTime timestamp = DateTime.Today.AddDays(-bars);
        decimal basePrice = 1000000m; // Large base to magnify precision issues

        for (int i = 0; i < bars; i++)
        {
            // Very small increments that exercise floating-point precision
            decimal close = basePrice + (i * 0.0000001m);
            quotes.Add(new Quote(
                Timestamp: timestamp.AddDays(i),
                Open: close,
                High: close + 0.0000001m,
                Low: close - 0.0000001m,
                Close: close,
                Volume: 1000m));
        }

        return quotes;
    }
}
