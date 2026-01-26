namespace StreamHubs;

[TestClass]
public class ElderRay : StreamHubTestBase, ITestQuoteObserver
{
    private const int lookbackPeriods = 13;
    private static readonly IReadOnlyList<ElderRayResult> expectedOriginal = Quotes.ToElderRay(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotes = Quotes.ToList();
        int length = quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(quotes.Take(20));

        // initialize observer
        ElderRayHub observer = quoteHub.ToElderRayHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<ElderRayResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(quotes[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);
        quotes.RemoveAt(removeAtIndex);

        IReadOnlyList<ElderRayResult> expectedRevised = quotes.ToElderRay(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ToStringOverride_ReturnsExpectedName()
    {
        ElderRayHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("ELDER-RAY(14)");
    }
}
