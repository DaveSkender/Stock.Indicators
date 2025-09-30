namespace BufferLists;

[TestClass]
public class Mama : BufferListTestBase
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MamaResult> series
       = Quotes.ToMama(fastLimit, slowLimit);

    private static readonly IReadOnlyList<MamaResult> reusableSeries
       = reusables.ToMama(fastLimit, slowLimit);

    [TestMethod]
    public void FromReusableSplit()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(reusableSeries.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = reusableSeries[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }

    [TestMethod]
    public void FromReusableItem()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(reusableSeries.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = reusableSeries[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        MamaList sut = new(fastLimit, slowLimit) { reusables };

        sut.Should().HaveCount(reusableSeries.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = reusableSeries[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }

    [TestMethod]
    public override void FromQuote()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(series.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = series[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        MamaList sut = new(fastLimit, slowLimit) { Quotes };

        IReadOnlyList<MamaResult> expected
            = Quotes.ToMama(fastLimit, slowLimit);

        sut.Should().HaveCount(expected.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = expected[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        MamaList sut = new(fastLimit, slowLimit);

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

        IReadOnlyList<MamaResult> expected = subset.ToMama(fastLimit, slowLimit);

        sut.Should().HaveCount(expected.Count);

        // Use tolerance-based comparison for floating-point precision
        for (int i = 0; i < sut.Count; i++)
        {
            MamaResult e = expected[i];
            MamaResult a = sut[i];

            a.Timestamp.Should().Be(e.Timestamp, $"Timestamp mismatch at index {i}");

            if (e.Mama is null)
            {
                a.Mama.Should().BeNull($"Expected null MAMA at index {i}");
            }
            else
            {
                a.Mama.Should().BeApproximately(e.Mama.Value, 1e-8,
                    $"MAMA value mismatch at index {i}. Expected: {e.Mama:F8}, Actual: {a.Mama:F8}");
            }

            if (e.Fama is null)
            {
                a.Fama.Should().BeNull($"Expected null FAMA at index {i}");
            }
            else
            {
                a.Fama.Should().BeApproximately(e.Fama.Value, 1e-8,
                    $"FAMA value mismatch at index {i}. Expected: {e.Fama:F8}, Actual: {a.Fama:F8}");
            }
        }
    }
}
