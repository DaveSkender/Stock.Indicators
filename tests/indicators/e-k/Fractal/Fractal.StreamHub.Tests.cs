namespace StreamHub;

[TestClass]
public class FractalHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        FractalHub observer = quoteHub
            .ToFractalHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<FractalResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
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

        // late arrivals
        quoteHub.Insert(quotesList[30]);  // rebuilds complete series
        quoteHub.Insert(quotesList[80]);  // rebuilds from insertion point

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // Ensure all fractals are calculated with full context
        observer.Rebuild(0);

        // time-series, for comparison
        IReadOnlyList<FractalResult> seriesList
           = quotesList
            .ToFractal();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverClose()
    {
        // simple test, just to check Close variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        FractalHub observer = quoteHub
            .ToFractalHub(endType: EndType.Close);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all fractals with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<FractalResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<FractalResult> seriesList
           = Quotes
            .ToFractal(endType: EndType.Close);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverDifferentSpans()
    {
        // test with different left and right spans

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with different spans
        FractalHub observer = quoteHub
            .ToFractalHub(3, 4);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all fractals with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<FractalResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<FractalResult> seriesList
           = Quotes
            .ToFractal(3, 4);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        FractalHub hub1 = new(new QuoteHub(), 2, EndType.HighLow);
        hub1.ToString().Should().Be("FRACTAL(2,2,HIGHLOW)");

        FractalHub hub2 = new(new QuoteHub(), 3, 4, EndType.Close);
        hub2.ToString().Should().Be("FRACTAL(3,4,CLOSE)");
    }
}
