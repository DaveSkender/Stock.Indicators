namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating Vortex Indicator hubs.
/// </summary>
public class VortexHub
    : StreamHub<IQuote, VortexResult>, IVortex
{

    private readonly Queue<(double Tr, double Pvm, double Nvm)> _buffer;
    private double _prevHigh;
    private double _prevLow;
    private double _prevClose;
    private bool _isInitialized;

    internal VortexHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Vortex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"VORTEX({lookbackPeriods})";

        _buffer = new Queue<(double, double, double)>(lookbackPeriods);
        _isInitialized = false;

        Reinitialize();
    }

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (VortexResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        // Handle first period - no calculations possible
        if (!_isInitialized)
        {
            _prevHigh = high;
            _prevLow = low;
            _prevClose = close;
            _isInitialized = true;

            return (new VortexResult(item.Timestamp), i);
        }

        // Calculate trend information
        double highMinusPrevClose = Math.Abs(high - _prevClose);
        double lowMinusPrevClose = Math.Abs(low - _prevClose);

        double tr = Math.Max(high - low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
        double pvm = Math.Abs(high - _prevLow);
        double nvm = Math.Abs(low - _prevHigh);

        // Update buffer with new values
        _buffer.Enqueue((tr, pvm, nvm));
        if (_buffer.Count > LookbackPeriods)
        {
            _buffer.Dequeue();
        }

        double? pvi = null;
        double? nvi = null;

        // Calculate vortex indicator when we have enough data
        if (_buffer.Count == LookbackPeriods)
        {
            double sumTr = 0;
            double sumPvm = 0;
            double sumNvm = 0;

            foreach ((double Tr, double Pvm, double Nvm) bufferItem in _buffer)
            {
                sumTr += bufferItem.Tr;
                sumPvm += bufferItem.Pvm;
                sumNvm += bufferItem.Nvm;
            }

            if (sumTr is not 0)
            {
                pvi = sumPvm / sumTr;
                nvi = sumNvm / sumTr;
            }
        }

        VortexResult result = new(
            Timestamp: item.Timestamp,
            Pvi: pvi,
            Nvi: nvi);

        _prevHigh = high;
        _prevLow = low;
        _prevClose = close;

        return (result, i);
    }

    /// <summary>
    /// Restores the buffer state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear buffer and state
        _buffer.Clear();
        _isInitialized = false;
        _prevHigh = 0;
        _prevLow = 0;
        _prevClose = 0;

        // Rebuild state from ProviderCache up to the rollback point
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;

        // Process first quote to initialize state
        if (targetIndex >= 0)
        {
            IQuote firstQuote = ProviderCache[0];
            _prevHigh = (double)firstQuote.High;
            _prevLow = (double)firstQuote.Low;
            _prevClose = (double)firstQuote.Close;
            _isInitialized = true;
        }

        // Rebuild buffer from quotes starting at index 1
        int startIdx = Math.Max(1, targetIndex + 1 - LookbackPeriods);
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            IQuote prevQuote = ProviderCache[p - 1];

            double high = (double)quote.High;
            double low = (double)quote.Low;
            double prevHigh = (double)prevQuote.High;
            double prevLow = (double)prevQuote.Low;
            double prevClose = (double)prevQuote.Close;

            // Calculate trend information
            double highMinusPrevClose = Math.Abs(high - prevClose);
            double lowMinusPrevClose = Math.Abs(low - prevClose);

            double tr = Math.Max(high - low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            double pvm = Math.Abs(high - prevLow);
            double nvm = Math.Abs(low - prevHigh);

            _buffer.Enqueue((tr, pvm, nvm));
        }

        // Update prev values to the last processed quote
        if (targetIndex >= 0)
        {
            IQuote lastQuote = ProviderCache[targetIndex];
            _prevHigh = (double)lastQuote.High;
            _prevLow = (double)lastQuote.Low;
            _prevClose = (double)lastQuote.Close;
        }
    }

}

public static partial class Vortex
{
    /// <summary>
    /// Converts the quote provider to a Vortex Indicator hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Vortex Indicator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static VortexHub ToVortexHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);
}
