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
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<Reusable> observer = provider
            .Use(CandlePart.HL2)
            .ToEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // stream results
        List<EmaResult> streamEma = observer
            .Results
            .ToList();

        // time-series, for comparison
        List<EmaResult> staticEma = Quotes
            .Use(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];
            Reusable e = observer.Provider.Results[i];

            // compare provider
            Assert.AreEqual(e.Timestamp, s.Timestamp);

            // compare series
            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Ema, r.Ema);
        }

        // confirm public interface
        Assert.AreEqual(observer.Cache.Count, observer.Results.Count);

        // confirm same length as provider cache
        Assert.AreEqual(observer.Cache.Count, provider.Results.Count);
    }
}
