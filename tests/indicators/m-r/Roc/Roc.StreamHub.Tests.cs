namespace StreamHub;

[TestClass]
public class RocHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 25; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        RocHub<Quote> observer = provider
            .ToRoc(20);

        // fetch initial results (early)
        IReadOnlyList<RocResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 25; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<RocResult> seriesList = quotesList.ToRoc(20);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int emaPeriods = 12;
        int rocPeriods = 20;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        RocHub<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToRoc(rocPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<RocResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<RocResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToRoc(rocPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int emaPeriods = 12;
        int rocPeriods = 20;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<RocResult> observer = provider
            .ToRoc(rocPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToRoc(rocPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        RocHub<Quote> observer = provider.ToRoc(20);

        observer.ToString().Should().Be("ROC(20)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ToRocStreamHubFromQuotes()
    {
        // Test basic usage
        RocHub<Quote> hub = Quotes.ToRocStreamHub(20);
        IReadOnlyList<RocResult> streamResults = hub.Results;

        // Compare with series results
        IReadOnlyList<RocResult> seriesResults = Quotes.ToRoc(20);

        // Assert
        streamResults.Should().HaveCount(Quotes.Count);
        streamResults.Should().BeEquivalentTo(seriesResults);

        hub.Unsubscribe();
    }

    [TestMethod]
    public void ToRocStreamHubDefaultLookback()
    {
        // Test with default lookback period
        RocHub<Quote> hub = Quotes.ToRocStreamHub();
        IReadOnlyList<RocResult> streamResults = hub.Results;

        // Compare with series results using default
        IReadOnlyList<RocResult> seriesResults = Quotes.ToRoc(14);

        // Assert
        streamResults.Should().HaveCount(Quotes.Count);
        streamResults.Should().BeEquivalentTo(seriesResults);

        hub.Unsubscribe();
    }

    [TestMethod]
    public void ToRocStreamHubNullQuotes()
    {
        // Test null quotes throws ArgumentNullException
        IReadOnlyList<Quote> nullQuotes = null;
        Action act = () => nullQuotes.ToRocStreamHub();
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ToRocStreamHubEmptyQuotes()
    {
        // Test empty quotes throws ArgumentException
        IReadOnlyList<Quote> emptyQuotes = new List<Quote>();
        Action act = () => emptyQuotes.ToRocStreamHub();
        act.Should().Throw<ArgumentException>()
            .WithMessage("Quotes list cannot be empty.*");
    }
}
