namespace Tests.Indicators.Stream;

[TestClass]
public class RenkoTests : StreamTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        decimal brickSize = 2.5m;
        EndType endType = EndType.Close;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        RenkoHub<Quote> observer = provider
            .ToRenko(brickSize, endType);

        // fetch initial results (early)
        IReadOnlyList<RenkoResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
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
        provider.Add(quotesList[80]);

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IEnumerable<RenkoResult> seriesList = quotesList
            .GetRenko(brickSize, endType);

        // assert, should equal series
        streamList.Should().HaveCount(112);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        decimal brickSize = 2.5m;
        EndType endType = EndType.Close;
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<RenkoResult> observer = provider
            .ToRenko(brickSize, endType)
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IEnumerable<SmaResult> seriesList = quotesList
            .GetRenko(brickSize, endType)
            .GetSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(112);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
