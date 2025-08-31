namespace Increments;

[TestClass]
public class Sma : BufferListTestBase
{
    private const int lookbackPeriods = 10;
    private const double Tolerance = 1E-8; // Financial precision tolerance

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<SmaResult> series
       = Quotes.ToSma(lookbackPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            if (sut[i].Sma.HasValue && series[i].Sma.HasValue)
            {
                sut[i].Sma.Should().BeApproximately(series[i].Sma!.Value, Tolerance);
            }
            else
            {
                sut[i].Sma.Should().Be(series[i].Sma);
            }
        }
    }

    [TestMethod]
    public void FromReusableItem()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            if (sut[i].Sma.HasValue && series[i].Sma.HasValue)
            {
                sut[i].Sma.Should().BeApproximately(series[i].Sma!.Value, Tolerance);
            }
            else
            {
                sut[i].Sma.Should().Be(series[i].Sma);
            }
        }
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        SmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            if (sut[i].Sma.HasValue && series[i].Sma.HasValue)
            {
                sut[i].Sma.Should().BeApproximately(series[i].Sma!.Value, Tolerance);
            }
            else
            {
                sut[i].Sma.Should().Be(series[i].Sma);
            }
        }
    }

    [TestMethod]
    public override void FromQuote()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            if (sut[i].Sma.HasValue && series[i].Sma.HasValue)
            {
                sut[i].Sma.Should().BeApproximately(series[i].Sma!.Value, Tolerance);
            }
            else
            {
                sut[i].Sma.Should().Be(series[i].Sma);
            }
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        SmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<SmaResult> series
            = Quotes.ToSma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating-point precision differences
        for (int i = 0; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().Be(series[i].Timestamp);
            if (sut[i].Sma.HasValue && series[i].Sma.HasValue)
            {
                sut[i].Sma.Should().BeApproximately(series[i].Sma!.Value, Tolerance);
            }
            else
            {
                sut[i].Sma.Should().Be(series[i].Sma);
            }
        }
    }
}