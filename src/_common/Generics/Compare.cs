namespace Skender.Stock.Indicators;

// COMPARE GENERIC TYPES

public static class CompareUtility
{
    // TODO: Unused, since it's too expensive.  Consider alternative override of native Equals, IEquatable operator

    // equal tuples
    internal static bool IsEqual(
        this (DateTime date, double value) a,
             (DateTime date, double value) b)
        => a.date == b.date
        && a.value == b.value;

    // equal series
    internal static bool IsEqual<TSeriesA, TSeriesB>(
            this TSeriesA a, TSeriesB b)
            where TSeriesA : ISeries
            where TSeriesB : ISeries
    {
        Type typeA = typeof(TSeriesA);
        Type typeB = typeof(TSeriesB);

        // nulls values are not expected
        if (a == null || b == null)
        {
            throw new ArgumentNullException(nameof(a), "Null equality comparison.");
        }

        // different types are not expected
        if (typeA != typeB)
        {
            throw new InvalidOperationException("Invalid equality comparison.");
        }

        // evaluate for quotes
        Type? quoteTypeA = typeA.GetInterface("IQuote");
        Type? quoteTypeB = typeB.GetInterface("IQuote");

        if (quoteTypeA != null && quoteTypeA.Name == "IQuote")
        {
            return IsEqualQuote((IQuote)a, (IQuote)b);
        }

        // evaluate for IReusableResult
        Type? resultTypeA = typeA.GetInterface("IReusableResult");
        Type? resultTypeB = typeB.GetInterface("IReusableResult");

        if (resultTypeA != null && resultTypeA.Name == "IReusableResult")
        {
            return IsEqualResult((IReusableResult)a, (IReusableResult)b);
        }

        // finally, compare dates
        return a.Date == b.Date;
    }

    private static bool IsEqualQuote<TQuote>(
        this TQuote a, TQuote b)
        where TQuote : IQuote
        => a.Date == b.Date
        && a.Open == b.Open
        && a.High == b.High
        && a.Low == b.Low
        && a.Close == b.Close
        && a.Volume == b.Volume;

    private static bool IsEqualResult<TReusableResult>(
        this TReusableResult a, TReusableResult b)
        where TReusableResult : IReusableResult
        => a.Date == b.Date
        && a.Value == b.Value;
}
