namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (STREAM HUB)

/// <summary>
/// Represents a stream hub for calculating the Alligator indicator.
/// </summary>
/// <inheritdoc cref="IAlligator"/>
public class AlligatorHub
   : StreamHub<IReusable, AlligatorResult>, IAlligator
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlligatorHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="jawPeriods">The number of periods for the jaw.</param>
    /// <param name="jawOffset">The offset for the jaw.</param>
    /// <param name="teethPeriods">The number of periods for the teeth.</param>
    /// <param name="teethOffset">The offset for the teeth.</param>
    /// <param name="lipsPeriods">The number of periods for the lips.</param>
    /// <param name="lipsOffset">The offset for the lips.</param>
    internal AlligatorHub(
        IChainProvider<IReusable> provider,
        int jawPeriods, int jawOffset,
        int teethPeriods, int teethOffset,
        int lipsPeriods, int lipsOffset)
        : base(provider)
    {
        Alligator.Validate(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

        JawPeriods = jawPeriods;
        JawOffset = jawOffset;
        TeethPeriods = teethPeriods;
        TeethOffset = teethOffset;
        LipsPeriods = lipsPeriods;
        LipsOffset = lipsOffset;

        hubName = $"ALLIGATOR({jawPeriods},{jawOffset},{teethPeriods},{teethOffset},{lipsPeriods},{lipsOffset})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the jaw.
    /// </summary>
    public int JawPeriods { get; init; }

    /// <summary>
    /// Gets the offset for the jaw.
    /// </summary>
    public int JawOffset { get; init; }

    /// <summary>
    /// Gets the number of periods for the teeth.
    /// </summary>
    public int TeethPeriods { get; init; }

    /// <summary>
    /// Gets the offset for the teeth.
    /// </summary>
    public int TeethOffset { get; init; }

    /// <summary>
    /// Gets the number of periods for the lips.
    /// </summary>
    public int LipsPeriods { get; init; }

    /// <summary>
    /// Gets the offset for the lips.
    /// </summary>
    public int LipsOffset { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AlligatorResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // calculate alligator's jaw, when in range
        if (i >= JawPeriods + JawOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Jaw is null)
            {
                double sum = 0;
                for (int p = i - JawPeriods - JawOffset + 1; p <= i - JawOffset; p++)
                {
                    sum += ProviderCache[p].Hl2OrValue();
                }

                jaw = sum / JawPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevJaw = Cache[i - 1].Jaw.Null2NaN();
                double newVal = ProviderCache[i - JawOffset].Hl2OrValue();

                jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
            }
        }

        // calculate alligator's teeth, when in range
        if (i >= TeethPeriods + TeethOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Teeth is null)
            {
                double sum = 0;
                for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                {
                    sum += ProviderCache[p].Hl2OrValue();
                }

                teeth = sum / TeethPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevTooth = Cache[i - 1].Teeth.Null2NaN();
                double newVal = ProviderCache[i - TeethOffset].Hl2OrValue();

                teeth = ((prevTooth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
            }
        }

        // calculate alligator's lips, when in range
        if (i >= LipsPeriods + LipsOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Lips is null)
            {
                double sum = 0;
                for (int p = i - LipsPeriods - LipsOffset + 1; p <= i - LipsOffset; p++)
                {
                    sum += ProviderCache[p].Hl2OrValue();
                }

                lips = sum / LipsPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevLips = Cache[i - 1].Lips.Null2NaN();
                double newVal = ProviderCache[i - LipsOffset].Hl2OrValue();

                lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
            }
        }

        // candidate result
        AlligatorResult r = new(
            item.Timestamp,
            jaw.NaN2Null(),
            teeth.NaN2Null(),
            lips.NaN2Null());

        return (r, i);
    }
}

public static partial class Alligator
{
    /// <summary>
    /// Converts a chain provider to an Alligator hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="jawPeriods">The number of periods for the jaw.</param>
    /// <param name="jawOffset">The offset for the jaw.</param>
    /// <param name="teethPeriods">The number of periods for the teeth.</param>
    /// <param name="teethOffset">The offset for the teeth.</param>
    /// <param name="lipsPeriods">The number of periods for the lips.</param>
    /// <param name="lipsOffset">The offset for the lips.</param>
    /// <returns>An Alligator hub.</returns>
    public static AlligatorHub ToAlligatorHub(
        this IChainProvider<IReusable> chainProvider,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3) => new(
            chainProvider,
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

    /// <summary>
    /// Creates an Alligator hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="jawPeriods">The number of periods for the jaw.</param>
    /// <param name="jawOffset">The offset for the jaw.</param>
    /// <param name="teethPeriods">The number of periods for the teeth.</param>
    /// <param name="teethOffset">The offset for the teeth.</param>
    /// <param name="lipsPeriods">The number of periods for the lips.</param>
    /// <param name="lipsOffset">The offset for the lips.</param>
    /// <returns>An instance of <see cref="AlligatorHub"/>.</returns>
    public static AlligatorHub ToAlligatorHub(
        this IReadOnlyList<IQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAlligatorHub(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);
    }
}
