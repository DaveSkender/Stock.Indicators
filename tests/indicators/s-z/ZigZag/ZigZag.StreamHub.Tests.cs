namespace StreamHub;

[TestClass]
public class ZigZagHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const EndType endType = EndType.Close;
    private const decimal percentChange = 3m;
    private static readonly IReadOnlyList<ZigZagResult> expectedOriginal = Quotes.ToZigZag(endType, percentChange);

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        ZigZagHub observer = quoteHub
            .ToZigZagHub(endType, percentChange);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // final results
        IReadOnlyList<ZigZagResult> streamList = observer.Results;

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 10;
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: ZigZag then SMA over its Value
        SmaHub observer = quoteHub
            .ToZigZagHub(endType, percentChange)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series parity
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToZigZag(endType, percentChange)
            .ToSma(smaPeriods);

        // assert count matches
        streamList.Should().HaveCount(length);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();

        ZigZagHub hub = new(quoteHub, EndType.Close, 5);
        hub.ToString().Should().Be("ZIGZAG(CLOSE,5)");
    }

    [TestMethod]
    public void RollbackValidation()
    {
        QuoteHub quoteHub = new();

        // Precondition: Normal quote stream with 502 expected entries
        ZigZagHub observer = quoteHub.ToZigZagHub(endType, percentChange);
        quoteHub.Add(Quotes);

        observer.Results.Should().HaveCount(502);
        observer.Results.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // Act: Remove a single historical value
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Assert: Observer should have 501 results and match revised series
        IReadOnlyList<ZigZagResult> expectedRevised = RevisedQuotes.ToZigZag(endType, percentChange);

        observer.Results.Should().HaveCount(501);
        observer.Results.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void HighLowType()
    {
        const EndType type = EndType.HighLow;
        const decimal pct = 3m;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        ZigZagHub observer = quoteHub.ToZigZagHub(type, pct);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<ZigZagResult> streamList = observer.Results;

        // time-series parity
        IReadOnlyList<ZigZagResult> seriesList = quotesList.ToZigZag(type, pct);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
