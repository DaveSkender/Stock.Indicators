namespace StreamHub;

[TestClass]
public class DpoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<DpoResult> expectedOriginal = Quotes.ToDpo(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        DpoHub observer = quoteHub.ToDpoHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<DpoResult> actuals = observer.Results;

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

        // late arrival - DPO maintains 1:1 input/output with last offset positions empty
        quoteHub.Insert(Quotes[80]);
        actuals.Should().HaveCount(Quotes.Count);
        actuals.Should().BeEquivalentTo(expectedOriginal, options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<DpoResult> expectedRevised = RevisedQuotes.ToDpo(lookbackPeriods);

        actuals.Should().HaveCount(RevisedQuotes.Count);
        actuals.Should().BeEquivalentTo(expectedRevised, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int dpoPeriods = 12;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        DpoHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToDpoHub(dpoPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<DpoResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<DpoResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToDpo(dpoPeriods);

        // assert - DPO maintains 1:1 with last offset positions empty
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int dpoPeriods = 20;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with intermediate DPO access
        DpoHub dpoHub = quoteHub.ToDpoHub(dpoPeriods);
        SmaHub observer = dpoHub.ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // DEBUG: Check DPO results at positions 17-26
        IReadOnlyList<DpoResult> dpoResults = dpoHub.Results;
        Console.WriteLine($"After Remove: DPO.Count = {dpoResults.Count}");
        for (int i = 17; i <= 26 && i < dpoResults.Count; i++)
        {
            Console.WriteLine($"DPO[{i}].Dpo = {dpoResults[i].Dpo}");
        }

        // final results
        IReadOnlyList<SmaResult> actuals
            = observer.Results;

        // time-series, for comparison (revised)
        // DPO maintains 1:1 output with input, last offset positions empty
        IReadOnlyList<DpoResult> seriesDpo = RevisedQuotes.ToDpo(dpoPeriods);
        IReadOnlyList<SmaResult> seriesList = seriesDpo.ToSma(smaPeriods);

        // DEBUG: Check Series DPO at positions 17-26
        Console.WriteLine($"Series DPO.Count = {seriesDpo.Count}");
        for (int i = 17; i <= 26 && i < seriesDpo.Count; i++)
        {
            Console.WriteLine($"Series DPO[{i}].Dpo = {seriesDpo[i].Dpo}, Series SMA[{i}].Sma = {seriesList[i].Sma}");
        }

        // assert - full count maintained with nulls at end
        actuals.Should().HaveCount(seriesList.Count);
        actuals.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        DpoHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("DPO(14)");
    }
}
