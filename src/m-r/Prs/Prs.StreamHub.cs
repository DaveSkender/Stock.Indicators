namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Price Relative Strength (PRS).
/// </summary>
public class PrsHub
    : ChainHub<IReusable, PrsResult>, IPrs
{
    private readonly IStreamObservable<IReusable> _providerBase;
    private IReadOnlyList<IReusable> _baseCache = null!;

    internal PrsHub(
        IChainProvider<IReusable> providerEval,
        IChainProvider<IReusable> providerBase,
        int lookbackPeriods) : base(providerEval)
    {
        _providerBase = providerBase ?? throw new ArgumentNullException(nameof(providerBase));
        _baseCache = _providerBase.GetCacheRef();
        LookbackPeriods = lookbackPeriods == int.MinValue ? int.MinValue : lookbackPeriods;

        if (lookbackPeriods is <= 0 and not int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Price Relative Strength.");
        }

        Name = lookbackPeriods == int.MinValue
            ? "PRS"
            : $"PRS({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (PrsResult result, int index)
        ToIndicator(IReusable itemEval, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(itemEval);

        // refresh base cache reference
        _baseCache = _providerBase.GetCacheRef();

        int i = indexHint ?? ProviderCache.IndexOf(itemEval, true);

        // validate matching timestamps
        if (i >= _baseCache.Count)
        {
            throw new InvalidQuotesException(
                nameof(itemEval), itemEval.Timestamp,
                "Base quotes should have at least as many records as Eval quotes for PRS.");
        }

        IReusable itemBase = _baseCache[i];

        if (itemEval.Timestamp != itemBase.Timestamp)
        {
            throw new InvalidQuotesException(
                nameof(itemEval), itemEval.Timestamp,
                "Timestamp sequence does not match.  "
              + "Price Relative requires matching dates in provided histories.");
        }

        // calculate PRS ratio
        double? prs = itemBase.Value == 0
            ? null
            : (itemEval.Value / itemBase.Value).NaN2Null();

        // calculate PRS percent if lookback is specified
        double? prsPercent = null;

        if (LookbackPeriods > 0 && i >= LookbackPeriods)
        {
            IReusable baseOld = _baseCache[i - LookbackPeriods];
            IReusable evalOld = ProviderCache[i - LookbackPeriods];

            if (baseOld.Value != 0 && evalOld.Value != 0)
            {
                double pctBase = (itemBase.Value - baseOld.Value) / baseOld.Value;
                double pctEval = (itemEval.Value - evalOld.Value) / evalOld.Value;

                prsPercent = (pctEval - pctBase).NaN2Null();
            }
        }

        PrsResult r = new(
            Timestamp: itemEval.Timestamp,
            Prs: prs,
            PrsPercent: prsPercent);

        return (r, i);
    }
}

public static partial class Prs
{
    /// <summary>
    /// Creates a PRS streaming hub from two chain providers.
    /// </summary>
    /// <param name="chainProviderEval">The evaluation chain provider.</param>
    /// <param name="chainProviderBase">The base chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the PRS% lookback calculation. Optional.</param>
    /// <returns>A PRS hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static PrsHub ToPrsHub(
        this IChainProvider<IReusable> chainProviderEval,
        IChainProvider<IReusable> chainProviderBase,
        int lookbackPeriods = int.MinValue)
    {
        ArgumentNullException.ThrowIfNull(chainProviderEval);
        ArgumentNullException.ThrowIfNull(chainProviderBase);

        return new PrsHub(chainProviderEval, chainProviderBase, lookbackPeriods);
    }
}
