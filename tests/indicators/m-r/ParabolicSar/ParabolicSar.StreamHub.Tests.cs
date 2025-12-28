namespace StreamHubs;

[TestClass]
public class ParabolicSarHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const double accelerationStep = 0.02;
    private const double maxAccelerationFactor = 0.2;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        ParabolicSarHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor);

        // fetch initial results (early)
        IReadOnlyList<ParabolicSarResult> streamList = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 50; i < length; i++)
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

        // Should match series after all quotes added
        IReadOnlyList<ParabolicSarResult> expectedOriginal = quotesList
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

        streamList.IsExactly(expectedOriginal);

        // delete
        quoteHub.Remove(quotesList[removeAtIndex]);
        quotesList.RemoveAt(removeAtIndex);

        // time-series, for comparison (revised)
        IReadOnlyList<ParabolicSarResult> seriesList = quotesList
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

        // assert, should equal series (revised)
        streamList.Should().HaveCount(501);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
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
        quoteHub.Remove(quotesList[removeAtIndex]);
        quotesList.RemoveAt(removeAtIndex);

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToParabolicSar(accelerationStep, maxAccelerationFactor)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(501);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        ParabolicSarHub hub = new(new QuoteHub(), 0.02, 0.2);
        hub.ToString().Should().Be("PSAR(0.02,0.2,0.02)");
    }

    [TestMethod]
    public void CustomInitialFactor()
    {
        const double customInitialFactor = 0.05;
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();
        quoteHub.Add(quotesList);

        // initialize observer with custom initial factor
        ParabolicSarHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor, customInitialFactor);

        // fetch results
        IReadOnlyList<ParabolicSarResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<ParabolicSarResult> seriesList = quotesList
            .ToParabolicSar(accelerationStep, maxAccelerationFactor, customInitialFactor);

        // assert, should equal series
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
