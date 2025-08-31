namespace Increments;

[TestClass]
public class Macd : BufferListTestBase
{
    private const int fastPeriods = 12;
    private const int slowPeriods = 26;
    private const int signalPeriods = 9;
    private const double Tolerance = 1E-8; // Financial precision tolerance

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MacdResult> series
       = Quotes.ToMacd(fastPeriods, slowPeriods, signalPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            CompareNullableDouble(sut[i].Macd, series[i].Macd);
            CompareNullableDouble(sut[i].Signal, series[i].Signal);
            CompareNullableDouble(sut[i].Histogram, series[i].Histogram);
            CompareNullableDouble(sut[i].FastEma, series[i].FastEma);
            CompareNullableDouble(sut[i].SlowEma, series[i].SlowEma);
        }
    }

    [TestMethod]
    public void FromReusableItem()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            CompareNullableDouble(sut[i].Macd, series[i].Macd);
            CompareNullableDouble(sut[i].Signal, series[i].Signal);
            CompareNullableDouble(sut[i].Histogram, series[i].Histogram);
            CompareNullableDouble(sut[i].FastEma, series[i].FastEma);
            CompareNullableDouble(sut[i].SlowEma, series[i].SlowEma);
        }
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            CompareNullableDouble(sut[i].Macd, series[i].Macd);
            CompareNullableDouble(sut[i].Signal, series[i].Signal);
            CompareNullableDouble(sut[i].Histogram, series[i].Histogram);
            CompareNullableDouble(sut[i].FastEma, series[i].FastEma);
            CompareNullableDouble(sut[i].SlowEma, series[i].SlowEma);
        }
    }

    [TestMethod]
    public override void FromQuote()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            CompareNullableDouble(sut[i].Macd, series[i].Macd);
            CompareNullableDouble(sut[i].Signal, series[i].Signal);
            CompareNullableDouble(sut[i].Histogram, series[i].Histogram);
            CompareNullableDouble(sut[i].FastEma, series[i].FastEma);
            CompareNullableDouble(sut[i].SlowEma, series[i].SlowEma);
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        MacdList sut = new(fastPeriods, slowPeriods, signalPeriods) { Quotes };

        IReadOnlyList<MacdResult> series
            = Quotes.ToMacd(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            CompareNullableDouble(sut[i].Macd, series[i].Macd);
            CompareNullableDouble(sut[i].Signal, series[i].Signal);
            CompareNullableDouble(sut[i].Histogram, series[i].Histogram);
            CompareNullableDouble(sut[i].FastEma, series[i].FastEma);
            CompareNullableDouble(sut[i].SlowEma, series[i].SlowEma);
        }
    }

    private static void CompareNullableDouble(double? actual, double? expected)
    {
        if (actual.HasValue && expected.HasValue)
        {
            actual.Should().BeApproximately(expected.Value, Tolerance);
        }
        else
        {
            actual.Should().Be(expected);
        }
    }
}