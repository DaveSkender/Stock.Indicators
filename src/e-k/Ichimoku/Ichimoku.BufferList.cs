namespace Skender.Stock.Indicators;

/// <summary>
/// Calculates Ichimoku Cloud indicator from incremental quote values with buffered state management.
/// </summary>
public class IchimokuList : BufferList<IchimokuResult>, IIncrementFromQuote, IIchimoku
{
    // Historical results buffer for lookback (needed for offset calculations)
    private readonly List<(DateTime Timestamp, decimal? TenkanSen, decimal? KijunSen)> _historicalResults;

    // Historical high/low buffer for Senkou Span B lookback (need to look back by offset)
    private readonly List<(decimal High, decimal Low)> _historicalHighLow;

    // Rolling window buffers for max/min calculations
    private readonly Queue<(decimal High, decimal Low)> _tenkanBuffer;
    private readonly Queue<(decimal High, decimal Low)> _kijunBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="IchimokuList"/> class.
    /// </summary>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="chikouOffset"/> is invalid.</exception>
    public IchimokuList(
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52,
        int senkouOffset = 26,
        int chikouOffset = 26)
    {
        Ichimoku.Validate(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);

        TenkanPeriods = tenkanPeriods;
        KijunPeriods = kijunPeriods;
        SenkouBPeriods = senkouBPeriods;
        SenkouOffset = senkouOffset;
        ChikouOffset = chikouOffset;

        // Initialize buffers
        _historicalResults = new List<(DateTime, decimal?, decimal?)>();
        _historicalHighLow = new List<(decimal, decimal)>();
        _tenkanBuffer = new Queue<(decimal, decimal)>(tenkanPeriods);
        _kijunBuffer = new Queue<(decimal, decimal)>(kijunPeriods);

        Name = $"ICHIMOKU({tenkanPeriods}, {kijunPeriods}, {senkouBPeriods}, {senkouOffset}, {chikouOffset})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IchimokuList"/> class with initial quotes.
    /// </summary>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public IchimokuList(
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset,
        IReadOnlyList<IQuote> quotes)
        : this(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset) => Add(quotes);

    /// <inheritdoc/>
    public int TenkanPeriods { get; init; }

    /// <inheritdoc/>
    public int KijunPeriods { get; init; }

    /// <inheritdoc/>
    public int SenkouBPeriods { get; init; }

    /// <inheritdoc/>
    public int SenkouOffset { get; init; }

    /// <inheritdoc/>
    public int ChikouOffset { get; init; }

    /// <summary>
    /// Adds a new quote to the Ichimoku list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Update rolling buffers using BufferUtilities
        _tenkanBuffer.Update(TenkanPeriods, (quote.High, quote.Low));
        _kijunBuffer.Update(KijunPeriods, (quote.High, quote.Low));

        // Store historical high/low for Senkou B lookback
        _historicalHighLow.Add((quote.High, quote.Low));

        // Calculate Tenkan-sen (conversion line)
        decimal? tenkanSen = null;
        if (_tenkanBuffer.Count == TenkanPeriods)
        {
            tenkanSen = CalculateMidpoint(_tenkanBuffer);
        }

        // Calculate Kijun-sen (base line)
        decimal? kijunSen = null;
        if (_kijunBuffer.Count == KijunPeriods)
        {
            kijunSen = CalculateMidpoint(_kijunBuffer);
        }

        // Store historical Tenkan and Kijun for offset lookback
        _historicalResults.Add((quote.Timestamp, tenkanSen, kijunSen));

        // Calculate Senkou Span A (leading span A)
        decimal? senkouSpanA = null;
        int senkouStartPeriod = Math.Max(
            2 * SenkouOffset,
            Math.Max(TenkanPeriods, KijunPeriods)) - 1;

        if (Count >= senkouStartPeriod)
        {
            if (SenkouOffset == 0)
            {
                senkouSpanA = (tenkanSen + kijunSen) / 2;
            }
            else if (Count >= SenkouOffset)
            {
                // Look back to get historical Tenkan and Kijun values
                int lookbackIndex = Count - SenkouOffset;
                if (lookbackIndex >= 0 && lookbackIndex < _historicalResults.Count)
                {
                    (DateTime _, decimal? historicalTenkan, decimal? historicalKijun) = _historicalResults[lookbackIndex];
                    senkouSpanA = (historicalTenkan + historicalKijun) / 2;
                }
            }
        }

        // Calculate Senkou Span B (leading span B)
        decimal? senkouSpanB = CalculateSenkouSpanB(Count);

        // Calculate Chikou Span (lagging span)
        // ChikouSpan for the current result will be null (can't see future in streaming)
        // Create and emit the result immediately
        IchimokuResult result = new(
            Timestamp: quote.Timestamp,
            TenkanSen: tenkanSen,
            KijunSen: kijunSen,
            SenkouSpanA: senkouSpanA,
            SenkouSpanB: senkouSpanB,
            ChikouSpan: null);

        AddInternal(result);

        // Update ChikouSpan for past result (retroactive update)
        // The result ChikouOffset periods back should have its ChikouSpan set to the current close
        if (Count > ChikouOffset)
        {
            int pastIndex = Count - ChikouOffset - 1;
            IchimokuResult pastResult = this[pastIndex];
            IchimokuResult updatedResult = pastResult with { ChikouSpan = quote.Close };
            UpdateInternal(pastIndex, updatedResult);
        }
    }

    /// <summary>
    /// Adds a list of quotes to the Ichimoku list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        // Pre-populate the future close buffer with all closes for Chikou Span calculation
        // This allows us to look ahead for Chikou values during batch processing
        List<decimal> allCloses = new(quotes.Count);
        for (int i = 0; i < quotes.Count; i++)
        {
            allCloses.Add(quotes[i].Close);
        }

        for (int i = 0; i < quotes.Count; i++)
        {
            IQuote quote = quotes[i];

            // Update rolling buffers using BufferUtilities
            _tenkanBuffer.Update(TenkanPeriods, (quote.High, quote.Low));
            _kijunBuffer.Update(KijunPeriods, (quote.High, quote.Low));

            // Store historical high/low for Senkou B lookback
            _historicalHighLow.Add((quote.High, quote.Low));

            // Calculate Tenkan-sen (conversion line)
            decimal? tenkanSen = null;
            if (_tenkanBuffer.Count == TenkanPeriods)
            {
                tenkanSen = CalculateMidpoint(_tenkanBuffer);
            }

            // Calculate Kijun-sen (base line)
            decimal? kijunSen = null;
            if (_kijunBuffer.Count == KijunPeriods)
            {
                kijunSen = CalculateMidpoint(_kijunBuffer);
            }

            // Store historical Tenkan and Kijun for offset lookback
            _historicalResults.Add((quote.Timestamp, tenkanSen, kijunSen));

            // Calculate Senkou Span A (leading span A)
            decimal? senkouSpanA = null;
            int senkouStartPeriod = Math.Max(
                2 * SenkouOffset,
                Math.Max(TenkanPeriods, KijunPeriods)) - 1;

            if (Count >= senkouStartPeriod)
            {
                if (SenkouOffset == 0)
                {
                    senkouSpanA = (tenkanSen + kijunSen) / 2;
                }
                else if (Count >= SenkouOffset)
                {
                    // Look back to get historical Tenkan and Kijun values
                    int lookbackIndex = Count - SenkouOffset;
                    if (lookbackIndex >= 0 && lookbackIndex < _historicalResults.Count)
                    {
                        (DateTime _, decimal? historicalTenkan, decimal? historicalKijun) = _historicalResults[lookbackIndex];
                        senkouSpanA = (historicalTenkan + historicalKijun) / 2;
                    }
                }
            }

            // Calculate Senkou Span B (leading span B)
            decimal? senkouSpanB = CalculateSenkouSpanB(Count);

            // Calculate Chikou Span (lagging span) - look ahead in the pre-populated list
            decimal? chikouSpan = null;
            int futureIndex = i + ChikouOffset;
            if (futureIndex < allCloses.Count)
            {
                chikouSpan = allCloses[futureIndex];
            }

            // Create and add the result
            IchimokuResult result = new(
                Timestamp: quote.Timestamp,
                TenkanSen: tenkanSen,
                KijunSen: kijunSen,
                SenkouSpanA: senkouSpanA,
                SenkouSpanB: senkouSpanB,
                ChikouSpan: chikouSpan);

            AddInternal(result);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();

        _historicalResults.Clear();
        _historicalHighLow.Clear();
        _tenkanBuffer.Clear();
        _kijunBuffer.Clear();
    }
    /// <summary>
    /// Calculates the midpoint (average of high and low) for a rolling window.
    /// </summary>
    /// <param name="buffer">The buffer containing high/low tuples.</param>
    /// <returns>The midpoint value.</returns>
    private static decimal CalculateMidpoint(Queue<(decimal High, decimal Low)> buffer)
    {
        decimal max = decimal.MinValue;
        decimal min = decimal.MaxValue;

        foreach ((decimal high, decimal low) in buffer)
        {
            if (high > max)
            {
                max = high;
            }

            if (low < min)
            {
                min = low;
            }
        }

        return (max + min) / 2;
    }

    /// <summary>
    /// Calculates the Senkou Span B value for the current position.
    /// Uses historical data from range [currentIndex - senkouOffset - senkouBPeriods + 1, currentIndex - senkouOffset].
    /// </summary>
    /// <param name="currentIndex">The current position in the results list.</param>
    /// <returns>The Senkou Span B value, or null if insufficient data.</returns>
    private decimal? CalculateSenkouSpanB(int currentIndex)
    {
        if (currentIndex < SenkouOffset + SenkouBPeriods - 1)
        {
            return null;
        }

        int endIndex = currentIndex - SenkouOffset;
        int startIndex = endIndex - SenkouBPeriods + 1;

        if (startIndex < 0 || endIndex >= _historicalHighLow.Count)
        {
            return null;
        }

        decimal max = decimal.MinValue;
        decimal min = decimal.MaxValue;

        for (int p = startIndex; p <= endIndex; p++)
        {
            (decimal high, decimal low) = _historicalHighLow[p];
            if (high > max)
            {
                max = high;
            }

            if (low < min)
            {
                min = low;
            }
        }

        return (max + min) / 2;
    }

    /// <summary>
    /// Removes oldest results from both the outer list and the nested historical buffers
    /// when the list exceeds <see cref="BufferList{TResult}.MaxListSize"/>.
    /// This prevents unbounded growth of the auxiliary historical caches.
    /// </summary>
    protected override void PruneList()
    {
        // Keep the historical buffers in sync with the main list
        int itemsToRemove = Count - MaxListSize;
        if (itemsToRemove > 0)
        {
            if (_historicalResults.Count > itemsToRemove)
            {
                _historicalResults.RemoveRange(0, itemsToRemove);
            }

            if (_historicalHighLow.Count > itemsToRemove)
            {
                _historicalHighLow.RemoveRange(0, itemsToRemove);
            }
        }

        // Call base implementation to prune the outer result list
        base.PruneList();
    }
}

/// <summary>
/// EXTENSION METHODS
/// </summary>
public static partial class Ichimoku
{
    /// <summary>
    /// Creates a buffer list for Ichimoku Cloud calculations.
    /// </summary>
    /// <param name="quotes">Historical price quotes.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line). Default is 9.</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line). Default is 26.</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B). Default is 52.</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset. Default is 26.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset. Default is 26.</param>
    /// <returns>An IchimokuList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static IchimokuList ToIchimokuList(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52,
        int senkouOffset = 26,
        int chikouOffset = 26)
        => new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset) { quotes };
}
