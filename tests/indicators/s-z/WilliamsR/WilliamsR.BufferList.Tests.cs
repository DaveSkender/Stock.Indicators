namespace BufferLists;

[TestClass]
public class WilliamsR : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<WilliamsResult> series
       = Quotes.ToWilliamsR(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        WilliamsRList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating point precision
        for (int i = 0; i < sut.Count; i++)
        {
            WilliamsResult actual = sut[i];
            WilliamsResult expected = series[i];
            
            actual.Timestamp.Should().Be(expected.Timestamp);
            
            if (expected.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(expected.WilliamsR.Value, 0.0001);
            }
            else
            {
                actual.WilliamsR.Should().Be(expected.WilliamsR);
            }
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        WilliamsRList sut = new(lookbackPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating point precision
        for (int i = 0; i < sut.Count; i++)
        {
            WilliamsResult actual = sut[i];
            WilliamsResult expected = series[i];
            
            actual.Timestamp.Should().Be(expected.Timestamp);
            
            if (expected.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(expected.WilliamsR.Value, 0.0001);
            }
            else
            {
                actual.WilliamsR.Should().Be(expected.WilliamsR);
            }
        }
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        WilliamsRList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        
        // Compare with tolerance for floating point precision
        for (int i = 0; i < sut.Count; i++)
        {
            WilliamsResult actual = sut[i];
            WilliamsResult expected = series[i];
            
            actual.Timestamp.Should().Be(expected.Timestamp);
            
            if (expected.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(expected.WilliamsR.Value, 0.0001);
            }
            else
            {
                actual.WilliamsR.Should().Be(expected.WilliamsR);
            }
        }
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        WilliamsRList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<WilliamsResult> expected = subset.ToWilliamsR(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        
        // Compare with tolerance for floating point precision
        for (int i = 0; i < sut.Count; i++)
        {
            WilliamsResult actual = sut[i];
            WilliamsResult exp = expected[i];
            
            actual.Timestamp.Should().Be(exp.Timestamp);
            
            if (exp.WilliamsR.HasValue && actual.WilliamsR.HasValue)
            {
                actual.WilliamsR.Should().BeApproximately(exp.WilliamsR.Value, 0.0001);
            }
            else
            {
                actual.WilliamsR.Should().Be(exp.WilliamsR);
            }
        }
    }

    [TestMethod]
    public void IncrementalConsistency()
    {
        // Test that incremental addition produces same results as batch
        WilliamsRList incremental = new(lookbackPeriods);
        WilliamsRList batch = new(lookbackPeriods) { Quotes };

        foreach (Quote quote in Quotes)
        {
            incremental.Add(quote);
        }

        incremental.Should().HaveCount(batch.Count);

        // Compare specific values to ensure accuracy
        for (int i = 0; i < incremental.Count; i++)
        {
            WilliamsResult inc = incremental[i];
            WilliamsResult bat = batch[i];

            inc.Timestamp.Should().Be(bat.Timestamp);

            if (inc.WilliamsR.HasValue && bat.WilliamsR.HasValue)
            {
                inc.WilliamsR.Should().BeApproximately(bat.WilliamsR.Value, 0.0001);
            }
            else
            {
                inc.WilliamsR.Should().Be(bat.WilliamsR);
            }
        }
    }

    [TestMethod]
    public void ParameterValidation()
    {
        // Test parameter validation
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new WilliamsRList(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new WilliamsRList(-1));
    }

    [TestMethod]
    public void BoundaryConditions()
    {
        // Test with minimal data
        WilliamsRList sut = new(5);
        List<Quote> minimal = Quotes.Take(5).ToList();

        foreach (Quote quote in minimal)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(minimal.Count);

        // Should have null values for initial periods
        for (int i = 0; i < 4; i++)
        {
            sut[i].WilliamsR.Should().BeNull();
        }

        // Should have a value at the lookback period boundary
        sut[4].WilliamsR.Should().NotBeNull();
    }

    [TestMethod]
    public void BufferListExtension()
    {
        // Test extension method
        WilliamsRList fromExtension = Quotes.ToWilliamsRList(lookbackPeriods);
        WilliamsRList fromConstructor = new(lookbackPeriods) { Quotes };

        fromExtension.Should().HaveCount(fromConstructor.Count);
        
        // Compare with tolerance for floating point precision
        for (int i = 0; i < fromExtension.Count; i++)
        {
            WilliamsResult ext = fromExtension[i];
            WilliamsResult con = fromConstructor[i];
            
            ext.Timestamp.Should().Be(con.Timestamp);
            
            if (ext.WilliamsR.HasValue && con.WilliamsR.HasValue)
            {
                ext.WilliamsR.Should().BeApproximately(con.WilliamsR.Value, 0.0001);
            }
            else
            {
                ext.WilliamsR.Should().Be(con.WilliamsR);
            }
        }
    }

    [TestMethod]
    public void BoundaryValues()
    {
        // Test that Williams %R stays within expected range [-100, 0]
        WilliamsRList sut = new(lookbackPeriods) { Quotes };

        foreach (WilliamsResult result in sut)
        {
            if (result.WilliamsR.HasValue)
            {
                result.WilliamsR.Value.Should().BeInRange(-100d, 0d);
            }
        }
    }
}
