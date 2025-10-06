namespace StreamHub;

[TestClass]
public class HmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 20;

    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        for (int i = 0; i < LookbackPeriods; i++)
        {
            provider.Add(quotesList[i]);
        }

        HmaHub<Quote> observer = provider.ToHma(LookbackPeriods);

        for (int i = LookbackPeriods; i < length; i++)
        {
            if (i == 80)
            {
                continue;
            }

            Quote quote = quotesList[i];
            provider.Add(quote);

            if (i is > 120 and < 126)
            {
                provider.Add(quote);
            }
        }

        provider.Insert(quotesList[80]);

        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        IReadOnlyList<HmaResult> streamList = observer.Results;
        IReadOnlyList<HmaResult> seriesList = quotesList.ToHma(LookbackPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        HmaHub<SmaResult> observer = provider
            .ToSma(smaPeriods)
            .ToHma(LookbackPeriods);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        IReadOnlyList<HmaResult> streamList = observer.Results;
        IReadOnlyList<HmaResult> seriesList = quotesList
            .ToSma(smaPeriods)
            .ToHma(LookbackPeriods);

        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        SmaHub<HmaResult> observer = provider
            .ToHma(LookbackPeriods)
            .ToSma(smaPeriods);

        for (int i = 0; i < length; i++)
        {
            if (i == 75)
            {
                continue;
            }

            Quote quote = quotesList[i];
            provider.Add(quote);

            if (i is > 180 and < 185)
            {
                provider.Add(quote);
            }
        }

        provider.Insert(quotesList[75]);

        provider.Remove(quotesList[300]);
        quotesList.RemoveAt(300);

        IReadOnlyList<SmaResult> streamList = observer.Results;
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToHma(LookbackPeriods)
            .ToSma(smaPeriods);

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void MidBufferMutationRehydrates()
    {
        List<Quote> quotesList = Quotes
            .Take(200)
            .ToList();

        QuoteHub<Quote> provider = new();
        HmaHub<Quote> observer = provider.ToHma(LookbackPeriods);

        for (int i = 0; i < quotesList.Count; i++)
        {
            provider.Add(quotesList[i]);
        }

        const int targetIndex = 120;
        observer.Results.Should().HaveCount(quotesList.Count);
        double? before = observer.Results[targetIndex].Hma;

        Quote original = quotesList[targetIndex];
        decimal delta = 1m;
        Quote mutated = original with {
            Open = original.Open + delta,
            High = original.High + delta,
            Low = original.Low + delta,
            Close = original.Close + delta
        };

        provider.Remove(original);
        provider.Insert(mutated);
        quotesList[targetIndex] = mutated;

        observer.Results.Should().HaveCount(quotesList.Count);
        double? after = observer.Results[targetIndex].Hma;
        after.Should().NotBe(before);

        IReadOnlyList<HmaResult> seriesList = quotesList.ToHma(LookbackPeriods);
        observer.Results.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void WarmupPeriodsRemainNull()
    {
        int sqrtPeriods = (int)Math.Sqrt(LookbackPeriods);
        int minSamples = LookbackPeriods - 1 + sqrtPeriods - 1;

        QuoteHub<Quote> provider = new();
        HmaHub<Quote> observer = provider.ToHma(LookbackPeriods);

        for (int i = 0; i < minSamples; i++)
        {
            provider.Add(Quotes[i]);
        }

        observer.Results.Should().HaveCount(minSamples);
        observer.Results.Should().OnlyContain(r => !r.Hma.HasValue);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void NaNInputProducesNullResult()
    {
        int injectionIndex = LookbackPeriods + 5;
        int totalCount = injectionIndex + (LookbackPeriods * 2);

        QuoteHub<SyntheticQuote> provider = new();
        HmaHub<SyntheticQuote> observer = provider.ToHma(LookbackPeriods);

        for (int i = 0; i < totalCount; i++)
        {
            DateTime timestamp = Quotes[i].Timestamp;
            double value = i == injectionIndex ? double.NaN : Quotes[i].Value;
            provider.Add(new SyntheticQuote(timestamp, value));
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
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        HmaHub<Quote> hub = new(new QuoteHub<Quote>(), LookbackPeriods);
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
