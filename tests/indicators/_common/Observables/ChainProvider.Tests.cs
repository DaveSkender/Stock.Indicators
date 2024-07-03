namespace Tests.Common.Observables;

[TestClass]
public class ChainProviderTests : TestBase, ITestChainProvider
{
    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<Reusable> observer = provider
            .Use(CandlePart.HL2)
            .ToEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // stream results
        IReadOnlyList<EmaResult> streamEma
            = observer.Results;

        // time-series, for comparison
        List<EmaResult> staticEma = Quotes
            .Use(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Ema.Should().Be(s.Ema);
            r.Should().Be(s);
        }

        // confirm public interface
        Assert.AreEqual(observer.CacheP.Count, observer.Results.Count);

        // confirm same length as provider cache
        Assert.AreEqual(observer.CacheP.Count, provider.Results.Count);
    }
}
