namespace StreamHub;

[TestClass]
public class SmiHub : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 13;
    private const int firstSmoothPeriods = 25;
    private const int secondSmoothPeriods = 2;
    private const int signalPeriods = 3;

    private static readonly IReadOnlyList<SmiResult> expectedOriginal
        = Quotes.ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        Skender.Stock.Indicators.SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

        // fetch initial results (early)
        IReadOnlyList<SmiResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
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

        IReadOnlyList<SmiResult> expectedRevised = RevisedQuotes.ToSmi(
            lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        // SMI emits IReusable results (SmiResult implements IReusable with Value = Smi),
        // so it can act as a chain provider for downstream indicators.

        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: SMI then EMA over its Value
        EmaHub observer = quoteHub
            .ToSmiHub(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods)
            .ToEmaHub(emaPeriods);

        // stream quotes
        foreach (Quote q in quotesList)
        {
            quoteHub.Add(q);
        }

        // results from stream
        IReadOnlyList<EmaResult> streamList = observer.Results;

        // time-series parity
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList, static o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();

        Skender.Stock.Indicators.SmiHub hub = new(
            quoteHub, lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
        hub.ToString().Should().Be($"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})");
    }
}
