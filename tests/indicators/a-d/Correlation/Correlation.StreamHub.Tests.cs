namespace StreamHub;

[TestClass]
public class CorrelationHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // Setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // Create two separate chains of the same type for correlation
        SmaHub<Quote> smaHubA = quoteHub.ToSma(10);
        SmaHub<Quote> smaHubB = quoteHub.ToSma(20);

        // Initialize correlation observer
        CorrelationHub<SmaResult> correlationHub = smaHubA.ToCorrelationHub(smaHubB, lookbackPeriods);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<CorrResult> streamList = correlationHub.Results;

        // Time-series, for comparison
        IReadOnlyList<SmaResult> seriesA = quotesList.ToSma(10);
        IReadOnlyList<SmaResult> seriesB = quotesList.ToSma(20);
        IReadOnlyList<CorrResult> seriesList = seriesA.ToCorrelation(seriesB, lookbackPeriods);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        correlationHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // Setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // Create chain with correlation of two SMAs, then RSI
        RsiHub<CorrResult> rsiHub = quoteHub
            .ToSma(10)
            .ToCorrelationHub(quoteHub.ToSma(20), lookbackPeriods)
            .ToRsiHub(14);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<RsiResult> streamList = rsiHub.Results;

        // Time-series, for comparison
        IReadOnlyList<RsiResult> seriesList = quotesList
            .ToSma(10)
            .ToCorrelation(quotesList.ToSma(20), lookbackPeriods)
            .ToRsi(14);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        rsiHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // Setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // Initialize chain - Correlation as provider feeding into EMA
        EmaHub<CorrResult> emaHub = quoteHub
            .ToSma(10)
            .ToCorrelationHub(quoteHub.ToSma(20), lookbackPeriods)
            .ToEmaHub(12);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<EmaResult> streamList = emaHub.Results;

        // Time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToSma(10)
            .ToCorrelation(quotesList.ToSma(20), lookbackPeriods)
            .ToEma(12);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        SmaHub<Quote> smaHubA = quoteHub.ToSma(10);
        SmaHub<Quote> smaHubB = quoteHub.ToSma(20);
        CorrelationHub<SmaResult> correlationHub = smaHubA.ToCorrelationHub(smaHubB, 20);

        correlationHub.ToString().Should().Be("CORRELATION(20)");

        correlationHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
