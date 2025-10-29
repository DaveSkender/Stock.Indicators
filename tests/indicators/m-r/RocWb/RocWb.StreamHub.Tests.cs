namespace StreamHub;

[TestClass]
public class RocWbHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
    private const int emaPeriods = 5;
    private const int stdDevPeriods = 5;

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 25; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        RocWbHub observer = quoteHub
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

        // fetch initial results (early)
        IReadOnlyList<RocWbResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<RocWbResult> seriesList = quotesList.ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int emaInnerPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        RocWbHub observer = quoteHub
            .ToEmaHub(emaInnerPeriods)
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<RocWbResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<RocWbResult> seriesList
           = quotesList
            .ToEma(emaInnerPeriods)
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int emaOuterPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods)
            .ToEmaHub(emaOuterPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods)
            .ToEma(emaOuterPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();
        RocWbHub observer = quoteHub.ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

        observer.ToString().Should().Be($"ROCWB({lookbackPeriods},{emaPeriods},{stdDevPeriods})");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
