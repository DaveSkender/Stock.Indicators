namespace StreamHub;

[TestClass]
public class RollingPivotsHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int windowPeriods = 20;
    private const int offsetPeriods = 0;
    private const PivotPointType pointType = PivotPointType.Standard;
    private readonly IReadOnlyList<RollingPivotsResult> expectedOriginal = Quotes.ToRollingPivots(windowPeriods, offsetPeriods, pointType);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(30));

        // initialize observer
        RollingPivotsHub observer = quoteHub.ToRollingPivotsHub(windowPeriods, offsetPeriods, pointType);

        // fetch initial results (early)
        IReadOnlyList<RollingPivotsResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 30; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);
        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<RollingPivotsResult> expectedRevised = RevisedQuotes.ToRollingPivots(windowPeriods, offsetPeriods, pointType);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        RollingPivotsHub hub = new QuoteHub().ToRollingPivotsHub(windowPeriods, offsetPeriods, pointType);
        hub.ToString().Should().Be("ROLLING-PIVOTS(20,0,STANDARD)");
    }

    [TestMethod]
    public void Standard()
    {
        // setup and run
        QuoteHub quoteHub = new();
        RollingPivotsHub sut = quoteHub.ToRollingPivotsHub(windowPeriods, offsetPeriods, pointType);

        foreach (IQuote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // compare to canonical Series results
        IReadOnlyList<RollingPivotsResult> series = Quotes.ToRollingPivots(windowPeriods, offsetPeriods, pointType);
        sut.Results.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithOffset()
    {
        const int testWindowPeriods = 15;
        const int testOffsetPeriods = 5;

        // setup and run
        QuoteHub quoteHub = new();
        RollingPivotsHub sut = quoteHub.ToRollingPivotsHub(testWindowPeriods, testOffsetPeriods, pointType);

        foreach (IQuote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // compare to canonical Series results
        IReadOnlyList<RollingPivotsResult> series = Quotes.ToRollingPivots(testWindowPeriods, testOffsetPeriods, pointType);
        sut.Results.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void DifferentPointTypes()
    {
        // Test with different pivot point types
        PivotPointType[] types = [PivotPointType.Standard, PivotPointType.Camarilla, PivotPointType.Demark, PivotPointType.Fibonacci, PivotPointType.Woodie];

        foreach (PivotPointType testType in types)
        {
            // setup and run
            QuoteHub quoteHub = new();
            RollingPivotsHub sut = quoteHub.ToRollingPivotsHub(windowPeriods, offsetPeriods, testType);

            foreach (IQuote q in Quotes)
            {
                quoteHub.Add(q);
            }

            // compare to canonical Series results
            IReadOnlyList<RollingPivotsResult> series = Quotes.ToRollingPivots(windowPeriods, offsetPeriods, testType);
            sut.Results.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());

            // cleanup
            sut.Unsubscribe();
            quoteHub.EndTransmission();
        }
    }

    [TestMethod]
    public void MinimumPeriods()
    {
        const int testWindowPeriods = 1;
        const int testOffsetPeriods = 0;

        // setup and run
        QuoteHub quoteHub = new();
        RollingPivotsHub sut = quoteHub.ToRollingPivotsHub(testWindowPeriods, testOffsetPeriods, pointType);

        foreach (IQuote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // compare to canonical Series results
        IReadOnlyList<RollingPivotsResult> series = Quotes.ToRollingPivots(testWindowPeriods, testOffsetPeriods, pointType);
        sut.Results.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void BadData()
    {
        // Test parameter validation
        QuoteHub quoteHub = new();

        Action act = () => quoteHub.ToRollingPivotsHub(0, 0, pointType);
        act.Should().Throw<ArgumentOutOfRangeException>();

        act = () => quoteHub.ToRollingPivotsHub(-1, 0, pointType);
        act.Should().Throw<ArgumentOutOfRangeException>();

        act = () => quoteHub.ToRollingPivotsHub(20, -1, pointType);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void InsufficientData()
    {
        const int testWindowPeriods = 50;
        const int testOffsetPeriods = 10;

        // setup with limited data
        QuoteHub quoteHub = new();
        RollingPivotsHub sut = quoteHub.ToRollingPivotsHub(testWindowPeriods, testOffsetPeriods, pointType);

        // Add fewer quotes than required
        foreach (IQuote q in Quotes.Take(40))
        {
            quoteHub.Add(q);
        }

        // All results should have null values
        sut.Results.Should().HaveCount(40);
        sut.Results.Should().AllSatisfy(static r => r.PP.Should().BeNull());

        // cleanup
        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
