namespace BufferLists;

[TestClass]
public class FisherTransform : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 10;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<FisherTransformResult> series
       = Quotes.ToFisherTransform(lookbackPeriods);

    [TestMethod]
    public override void AddQuotes()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(series, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        FisherTransformList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<FisherTransformResult> series
            = Quotes.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(series, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        FisherTransformList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(series, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        FisherTransformList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<FisherTransformResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<FisherTransformResult> expected = subset.ToFisherTransform(lookbackPeriods);

        FisherTransformList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        // FisherTransform uses recursive calculations which accumulate floating-point rounding differences
        // Use tolerance of 1e-13 to account for acceptable precision loss in recursive formulas
        sut.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-13))
            .WhenTypeIs<double>()
            .Using<double?>(ctx => {
                if (ctx.Expectation.HasValue && ctx.Subject.HasValue)
                {
                    ctx.Subject.Value.Should().BeApproximately(ctx.Expectation.Value, 1e-13);
                }
                else
                {
                    ctx.Subject.Should().Be(ctx.Expectation);
                }
            })
            .WhenTypeIs<double?>());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        // For FisherTransform with IReusable, we're using Close values
        // whereas with IQuote we use HL2, so we need to compare to reusable series
        IReadOnlyList<FisherTransformResult> reusableSeries = reusables.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(reusableSeries, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        // For FisherTransform with IReusable, we're using Close values
        // whereas with IQuote we use HL2, so we need to compare to reusable series
        IReadOnlyList<FisherTransformResult> reusableSeries = reusables.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(reusableSeries, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        FisherTransformList sut = new(lookbackPeriods) { reusables };

        // For FisherTransform with IReusable, we're using Close values
        // whereas with IQuote we use HL2, so we need to compare to reusable series
        IReadOnlyList<FisherTransformResult> reusableSeries = reusables.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(reusableSeries, options => options.WithStrictOrdering());
    }
}
