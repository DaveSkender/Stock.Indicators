namespace StreamHub;

[TestClass]
public class Smi : StreamHubTestBase, ITestQuoteObserver
{
    private const int lookbackPeriods = 13;
    private const int firstSmoothPeriods = 25;
    private const int secondSmoothPeriods = 2;
    private const int signalPeriods = 3;

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // fetch initial results (early)
        IReadOnlyList<SmiResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(quotesList[80]);

        IReadOnlyList<SmiResult> expectedOriginal = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(quotesList[removeAtIndex]);

        IReadOnlyList<SmiResult> expectedRevised = RevisedQuotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        SmiHub hub = new(
            new QuoteHub(),
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        hub.ToString().Should().Be($"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})");
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub with incremental updates
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        // close observations
        quoteHub.EndTransmission();

        // verify consistency
        IReadOnlyList<SmiResult> expected = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        observer.Cache.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void Properties()
    {
        const int testLookbackPeriods = 21;
        const int testFirstSmoothPeriods = 30;
        const int testSecondSmoothPeriods = 5;
        const int testSignalPeriods = 7;

        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            testLookbackPeriods,
            testFirstSmoothPeriods,
            testSecondSmoothPeriods,
            testSignalPeriods);

        // verify properties
        observer.LookbackPeriods.Should().Be(testLookbackPeriods);
        observer.FirstSmoothPeriods.Should().Be(testFirstSmoothPeriods);
        observer.SecondSmoothPeriods.Should().Be(testSecondSmoothPeriods);
        observer.SignalPeriods.Should().Be(testSignalPeriods);
        observer.ToString().Should().Be($"SMI({testLookbackPeriods},{testFirstSmoothPeriods},{testSecondSmoothPeriods},{testSignalPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub();

        // verify default properties
        observer.LookbackPeriods.Should().Be(13);
        observer.FirstSmoothPeriods.Should().Be(25);
        observer.SecondSmoothPeriods.Should().Be(2);
        observer.SignalPeriods.Should().Be(3);
        observer.ToString().Should().Be("SMI(13,25,2,3)");
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test that streaming produces accurate results compared to batch processing
        List<Quote> quotesList = Quotes.ToList();

        // streaming calculation
        QuoteHub quoteHub = new();
        SmiHub streamObserver = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        quoteHub.EndTransmission();

        // batch calculation
        IReadOnlyList<SmiResult> batchResults = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // compare results with strict ordering
        streamObserver.Cache.Should().HaveCount(batchResults.Count);
        streamObserver.Cache.Should().BeEquivalentTo(batchResults, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void BatchProcessing()
    {
        // Test batch processing with all quotes added at once
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // add all quotes at once
        quoteHub.Add(Quotes);
        quoteHub.EndTransmission();

        // verify against static series calculation
        IReadOnlyList<SmiResult> expected = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        observer.Cache.Should().HaveCount(Quotes.Count);
        observer.Cache.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
