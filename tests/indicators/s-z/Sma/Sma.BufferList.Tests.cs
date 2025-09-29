namespace BufferLists;

[TestClass]
public class Sma : BufferListTestBase
{
    private const int lookbackPeriods = 14;
    private const double BufferPrecision = 1E-12; // Slightly larger tolerance for buffer algorithms

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

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = series[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
            }
        }
    }

    [TestMethod]
    public void FromReusableItem()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = series[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
            }
        }
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        SmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = series[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
            }
        }
    }

    [TestMethod]
    public override void FromQuote()
    {
        SmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = series[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
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

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = series[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
            }
        }
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        SmaList sut = new(lookbackPeriods);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<SmaResult> expected = subset.ToSma(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);

        for (int i = 0; i < sut.Count; i++)
        {
            SmaResult e = expected[i];
            SmaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp);
            if (e.Sma is null)
            {
                a.Sma.Should().BeNull();
            }
            else
            {
                a.Sma.Should().BeApproximately(e.Sma.Value, BufferPrecision);
            }
        }
    }
}
