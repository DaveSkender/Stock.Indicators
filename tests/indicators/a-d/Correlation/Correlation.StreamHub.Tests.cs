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
        QuoteHub<Quote> provider = new();

        // Create two separate chains of the same type for correlation
        SmaHub<Quote> chainA = provider.ToSma(10);
        SmaHub<Quote> chainB = provider.ToSma(20);

        // Initialize correlation observer
        CorrelationHub<SmaResult> observer = chainA.ToCorrelation(chainB, lookbackPeriods);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<CorrResult> streamList = observer.Results;

        // Time-series, for comparison
        IReadOnlyList<SmaResult> seriesA = quotesList.ToSma(10);
        IReadOnlyList<SmaResult> seriesB = quotesList.ToSma(20);
        IReadOnlyList<CorrResult> seriesList = seriesA.ToCorrelation(seriesB, lookbackPeriods);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // Setup quote provider
        QuoteHub<Quote> provider = new();

        // Create chain with correlation of two SMAs, then RSI
        RsiHub<CorrResult> observer = provider
            .ToSma(10)
            .ToCorrelation(provider.ToSma(20), lookbackPeriods)
            .ToRsi(14);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<RsiResult> streamList = observer.Results;

        // Time-series, for comparison
        IReadOnlyList<RsiResult> seriesList = quotesList
            .ToSma(10)
            .ToCorrelation(quotesList.ToSma(20), lookbackPeriods)
            .ToRsi(14);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;
        int lookbackPeriods = 20;

        // Setup quote provider
        QuoteHub<Quote> provider = new();

        // Create chain with correlation feeding into EMA
        EmaHub<CorrResult> observer = provider
            .ToSma(10)
            .ToCorrelation(provider.ToSma(20), lookbackPeriods)
            .ToEma(12);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<EmaResult> streamList = observer.Results;

        // Time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToSma(10)
            .ToCorrelation(quotesList.ToSma(20), lookbackPeriods)
            .ToEma(12);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        SmaHub<Quote> chainA = provider.ToSma(10);
        SmaHub<Quote> chainB = provider.ToSma(20);
        CorrelationHub<SmaResult> observer = chainA.ToCorrelation(chainB, 20);

        observer.ToString().Should().Be("CORRELATION(20)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
