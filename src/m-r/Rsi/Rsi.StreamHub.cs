namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Relative Strength Index (RSI) indicator.
/// </summary>
public class RsiHub
    : ChainProvider<IReusable, RsiResult>, IRsi
{
    private readonly string hubName;
    private double avgGain;
    private double avgLoss;

    /// <summary>
    /// Initializes a new instance of the <see cref="RsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal RsiHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Rsi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"RSI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? rsi = null;
        double currentValue = item.Value;

        // Get previous value for gain/loss calculation
        double prevValue = i > 0 ? ProviderCache[i - 1].Value : double.NaN;

        // Calculate current gain/loss
        double gain;
        double loss;
        if (!double.IsNaN(currentValue) && !double.IsNaN(prevValue))
        {
            gain = currentValue > prevValue ? currentValue - prevValue : 0;
            loss = currentValue < prevValue ? prevValue - currentValue : 0;
        }
        else
        {
            gain = loss = double.NaN;
        }

        // Restore state if needed (after rollback)
        if (i > LookbackPeriods && (double.IsNaN(avgGain) || double.IsNaN(avgLoss)))
        {
            // Recalculate state by replaying from the first calculable position
            double tempAvgGain = double.NaN;
            double tempAvgLoss = double.NaN;

            for (int p = LookbackPeriods; p < i; p++)
            {
                double pPrevVal = ProviderCache[p - 1].Value;
                double pCurrVal = ProviderCache[p].Value;
                double pGain;
                double pLoss;
                if (!double.IsNaN(pCurrVal) && !double.IsNaN(pPrevVal))
                {
                    pGain = pCurrVal > pPrevVal ? pCurrVal - pPrevVal : 0;
                    pLoss = pCurrVal < pPrevVal ? pPrevVal - pCurrVal : 0;
                }
                else
                {
                    pGain = pLoss = double.NaN;
                }

                // Initialize or update averages
                if (p == LookbackPeriods && (double.IsNaN(tempAvgGain) || double.IsNaN(tempAvgLoss)))
                {
                    // Initial SMA calculation
                    double sumGain = 0;
                    double sumLoss = 0;

                    for (int q = p - LookbackPeriods + 1; q <= p; q++)
                    {
                        double qPrevVal = ProviderCache[q - 1].Value;
                        double qCurrVal = ProviderCache[q].Value;
                        double qGain;
                        double qLoss;
                        if (!double.IsNaN(qCurrVal) && !double.IsNaN(qPrevVal))
                        {
                            qGain = qCurrVal > qPrevVal ? qCurrVal - qPrevVal : 0;
                            qLoss = qCurrVal < qPrevVal ? qPrevVal - qCurrVal : 0;
                        }
                        else
                        {
                            qGain = qLoss = double.NaN;
                        }

                        sumGain += qGain;
                        sumLoss += qLoss;
                    }

                    tempAvgGain = sumGain / LookbackPeriods;
                    tempAvgLoss = sumLoss / LookbackPeriods;
                }
                else if (p > LookbackPeriods && !double.IsNaN(tempAvgGain) && !double.IsNaN(tempAvgLoss))
                {
                    if (!double.IsNaN(pGain))
                    {
                        // EMA-style update
                        tempAvgGain = ((tempAvgGain * (LookbackPeriods - 1)) + pGain) / LookbackPeriods;
                        tempAvgLoss = ((tempAvgLoss * (LookbackPeriods - 1)) + pLoss) / LookbackPeriods;
                    }
                    else
                    {
                        tempAvgGain = tempAvgLoss = double.NaN;
                    }
                }
            }

            avgGain = tempAvgGain;
            avgLoss = tempAvgLoss;
        }

        // Re/initialize average gain/loss when needed
        if (i >= LookbackPeriods && (double.IsNaN(avgGain) || double.IsNaN(avgLoss)))
        {
            double sumGain = 0;
            double sumLoss = 0;

            // Sum gains and losses over lookback period
            // Note: NaN values will contaminate the sum, as intended
            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                double pPrevVal = ProviderCache[p - 1].Value;
                double pCurrVal = ProviderCache[p].Value;
                double pGain;
                double pLoss;
                if (!double.IsNaN(pCurrVal) && !double.IsNaN(pPrevVal))
                {
                    pGain = pCurrVal > pPrevVal ? pCurrVal - pPrevVal : 0;
                    pLoss = pCurrVal < pPrevVal ? pPrevVal - pCurrVal : 0;
                }
                else
                {
                    pGain = pLoss = double.NaN;
                }

                sumGain += pGain;
                sumLoss += pLoss;
            }

            avgGain = sumGain / LookbackPeriods;
            avgLoss = sumLoss / LookbackPeriods;

            rsi = !double.IsNaN(avgGain / avgLoss)
                ? avgLoss > 0
                  ? 100 - (100 / (1 + (avgGain / avgLoss)))
                  : 100
                : null;
        }
        // Calculate RSI incrementally
        else if (i > LookbackPeriods && !double.IsNaN(avgGain) && !double.IsNaN(avgLoss))
        {
            if (!double.IsNaN(gain))
            {
                avgGain = ((avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
                avgLoss = ((avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;

                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    rsi = 100;
                }
            }
            else
            {
                // Reset state if we hit NaN
                avgGain = double.NaN;
                avgLoss = double.NaN;
            }
        }

        // candidate result
        RsiResult r = new(
            Timestamp: item.Timestamp,
            Rsi: rsi.NaN2Null());

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state - will be recalculated during rebuild
        avgGain = double.NaN;
        avgLoss = double.NaN;
    }
}

public static partial class Rsi
{
    /// <summary>
    /// Creates an RSI streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RsiHub ToRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Rsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RsiHub"/>.</returns>
    public static RsiHub ToRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRsiHub(lookbackPeriods);
    }

}
