namespace StreamHubs;

[TestClass]
public class HmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 20;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        HmaHub observer = quoteHub.ToHmaHub(LookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<HmaResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<HmaResult> expectedOriginal = Quotes.ToHma(LookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);
        IReadOnlyList<HmaResult> expectedRevised = RevisedQuotes.ToHma(LookbackPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        HmaHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToHmaHub(LookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<HmaResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<HmaResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToHma(LookbackPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int hmaPeriods = LookbackPeriods;
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToHmaHub(hmaPeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < quotesCount; i++)
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

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToHma(hmaPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void MidBufferMutationRehydrates()
    {
        List<Quote> quotesList = Quotes
            .Take(200)
            .ToList();

        QuoteHub quoteHub = new();
        HmaHub observer = quoteHub.ToHmaHub(LookbackPeriods);

        for (int i = 0; i < quotesList.Count; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        const int targetIndex = 120;
        observer.Results.Should().HaveCount(quotesList.Count);
        double? before = observer.Results[targetIndex].Hma;

        Quote original = quotesList[targetIndex];
        const decimal delta = 1m;
        Quote mutated = original with {
            Open = original.Open + delta,
            High = original.High + delta,
            Low = original.Low + delta,
            Close = original.Close + delta
        };

        quoteHub.Remove(original);
        quoteHub.Insert(mutated);
        quotesList[targetIndex] = mutated;

        observer.Results.Should().HaveCount(quotesList.Count);
        double? after = observer.Results[targetIndex].Hma;
        after.Should().NotBe(before);

        IReadOnlyList<HmaResult> seriesList = quotesList.ToHma(LookbackPeriods);
        observer.Results.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WarmupPeriodsRemainNull()
    {
        int sqrtPeriods = (int)Math.Sqrt(LookbackPeriods);
        int minSamples = LookbackPeriods - 1 + sqrtPeriods - 1;

        QuoteHub quoteHub = new();
        HmaHub observer = quoteHub.ToHmaHub(LookbackPeriods);

        for (int i = 0; i < minSamples; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        observer.Results.Should().HaveCount(minSamples);
        observer.Results.Should().OnlyContain(static r => !r.Hma.HasValue);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void NaNInputProducesNullResult()
    {
        const int injectionIndex = LookbackPeriods + 5;
        const int totalCount = injectionIndex + (LookbackPeriods * 2);

        QuoteHub quoteHub = new();
        HmaHub observer = quoteHub.ToHmaHub(LookbackPeriods);

        for (int i = 0; i < totalCount; i++)
        {
            DateTime timestamp = Quotes[i].Timestamp;
            double value = i == injectionIndex ? double.NaN : Quotes[i].Value;
            quoteHub.Add(new SyntheticQuote(timestamp, value));
        }

        observer.Results[injectionIndex].Hma.Should().BeNull();

        int? recoveryIndex = null;

        for (int i = injectionIndex + 1; i < observer.Results.Count; i++)
        {
            if (observer.Results[i].Hma.HasValue)
            {
                recoveryIndex = i;
                break;
            }
        }

        recoveryIndex.Should().NotBeNull();

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        HmaHub hub = new(new QuoteHub(), LookbackPeriods);
        hub.ToString().Should().Be($"HMA({LookbackPeriods})");
    }

    private sealed record SyntheticQuote(DateTime Timestamp, double RawValue) : IQuote
    {
        public decimal Open => 0m;

        public decimal High => 0m;

        public decimal Low => 0m;

        public decimal Close => 0m;

        public decimal Volume => 0m;

        public double Value => RawValue;
    }
}
