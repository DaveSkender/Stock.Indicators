namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Fisher Transform indicator using a stream hub.
/// </summary>
public class FisherTransformHub
    : ChainProvider<IReusable, FisherTransformResult>, IFisherTransform
{
    private readonly string hubName;

    /// <summary>
    /// State arrays for Fisher Transform algorithm
    /// These arrays grow with each added value to support indexed lookback access
    /// </summary>
    private readonly List<double> xv = []; // value transform (intermediate state)

    /// <summary>
    /// Initializes a new instance of the <see cref="FisherTransformHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 10.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal FisherTransformHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods = 10) : base(provider)
    {
        FisherTransform.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"FISHER({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (FisherTransformResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // NOTE: Fisher Transform has internal state (xv).  We maintain state arrays that are
        // truncated on rebuild events.  See RollbackState() override below.

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Ensure arrays have enough capacity
        while (xv.Count <= i)
        {
            xv.Add(0);
        }

        // prefer HL2 when source is an IQuote
        double currentValue = item.Hl2OrValue();

        // find min/max in lookback window
        double minPrice = currentValue;
        double maxPrice = currentValue;

        for (int p = Math.Max(i - LookbackPeriods + 1, 0); p <= i; p++)
        {
            double priceValue = ProviderCache[p].Hl2OrValue();
            minPrice = Math.Min(priceValue, minPrice);
            maxPrice = Math.Max(priceValue, maxPrice);
        }

        double? fisher;
        double? trigger = null;

        if (i > 0)
        {
            // calculate current xv
            xv[i] = maxPrice - minPrice != 0
                ? (0.33 * 2 * (((currentValue - minPrice) / (maxPrice - minPrice)) - 0.5))
                      + (0.67 * xv[i - 1])
                : 0;

            // limit xv to prevent log issues
            xv[i] = xv[i] > 0.99 ? 0.999 : xv[i];
            xv[i] = xv[i] < -0.99 ? -0.999 : xv[i];

            // calculate Fisher Transform
            fisher = ((0.5 * Math.Log((1 + xv[i]) / (1 - xv[i])))
                  + (0.5 * Cache[i - 1].Fisher)).NaN2Null();

            trigger = Cache[i - 1].Fisher;
        }
        else
        {
            xv[i] = 0;
            fisher = 0;
        }

        // candidate result
        FisherTransformResult r = new(
            Timestamp: item.Timestamp,
            Fisher: fisher,
            Trigger: trigger);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            xv.Clear();
            return;
        }

        if (index < xv.Count)
        {
            int removeCount = xv.Count - index;
            xv.RemoveRange(index, removeCount);
        }
    }
}


public static partial class FisherTransform
{
    /// <summary>
    /// Creates a Fisher Transform streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 10.</param>
    /// <returns>A Fisher Transform hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static FisherTransformHub ToFisherTransformHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 10)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Fisher Transform hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation. Default is 10.</param>
    /// <returns>An instance of <see cref="FisherTransformHub"/>.</returns>
    public static FisherTransformHub ToFisherTransformHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFisherTransformHub(lookbackPeriods);
    }
}
