namespace Observables;

/// <summary>
/// Verifies the O(1) near-tail rollback fast path (RollbackRing snapshots)
/// and its deep-rollback fallback produce Series-exact results for stateful
/// hubs. RSI is the reference implementation; shallow corrections restore
/// state from ring snapshots while corrections deeper than the ring's
/// capacity fall back to full state replay.
/// </summary>
[TestClass]
public class RollbackRingState : TestBase
{
    [TestMethod]
    public void ShallowCorrection_RestoresFromSnapshot_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(120).ToList();

        QuoteHub quoteHub = new();
        RsiHub observer = quoteHub.ToRsiHub(14);

        quoteHub.Add(quotes);

        // correct a recent bar (well within ring capacity)
        Quote corrected = quotes[117] with { Close = quotes[117].Close + 2m };
        quotes[117] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToRsi(14));
    }

    [TestMethod]
    public void DeepCorrection_FallsBackToReplay_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(200).ToList();

        QuoteHub quoteHub = new();
        RsiHub observer = quoteHub.ToRsiHub(14);

        quoteHub.Add(quotes);

        // correct a bar far beyond the ring capacity (32 snapshots)
        Quote corrected = quotes[60] with { Close = quotes[60].Close + 2m };
        quotes[60] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToRsi(14));
    }

    [TestMethod]
    public void FormingBarChurn_RepeatedTailCorrections_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(80).ToList();

        QuoteHub quoteHub = new();
        RsiHub observer = quoteHub.ToRsiHub(14);

        quoteHub.Add(quotes);

        // emulate an aggregator re-sending the forming bar on every tick;
        // more updates than the ring capacity, all on one timestamp
        Quote formingBar = quotes[^1];
        for (int update = 1; update <= 40; update++)
        {
            formingBar = formingBar with { Close = formingBar.Close + 0.25m };
            quoteHub.Add(formingBar);
        }

        quotes[^1] = formingBar;
        observer.Results.IsExactly(quotes.ToRsi(14));
    }

    [TestMethod]
    public void SequentialCorrections_AcrossRecentBars_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(100).ToList();

        QuoteHub quoteHub = new();
        RsiHub observer = quoteHub.ToRsiHub(14);

        quoteHub.Add(quotes);

        // several distinct recent bars corrected in sequence: each rollback
        // restores from a snapshot recorded by the previous replay
        for (int i = 95; i < 100; i++)
        {
            Quote corrected = quotes[i] with { Close = quotes[i].Close + 1m };
            quotes[i] = corrected;
            quoteHub.Add(corrected);
        }

        observer.Results.IsExactly(quotes.ToRsi(14));
    }

    [TestMethod]
    public void ShallowCorrection_AdxHub_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(150).ToList();

        QuoteHub quoteHub = new();
        AdxHub observer = quoteHub.ToAdxHub(14);

        quoteHub.Add(quotes);

        Quote corrected = quotes[147] with { Close = quotes[147].Close + 2m };
        quotes[147] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToAdx(14));
    }

    [TestMethod]
    public void ShallowCorrection_SuperTrendHub_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(150).ToList();

        QuoteHub quoteHub = new();
        SuperTrendHub observer = quoteHub.ToSuperTrendHub(10, 3);

        quoteHub.Add(quotes);

        Quote corrected = quotes[147] with { Close = quotes[147].Close + 2m };
        quotes[147] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToSuperTrend(10, 3));
    }

    [TestMethod]
    public void ShallowCorrection_KvoHub_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(150).ToList();

        QuoteHub quoteHub = new();
        KvoHub observer = quoteHub.ToKvoHub(34, 55, 13);

        quoteHub.Add(quotes);

        Quote corrected = quotes[147] with { Close = quotes[147].Close + 2m };
        quotes[147] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToKvo(34, 55, 13));
    }

    [TestMethod]
    public void ShallowCorrection_ParabolicSarHub_MatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(150).ToList();

        QuoteHub quoteHub = new();
        ParabolicSarHub observer = quoteHub.ToParabolicSarHub(0.02, 0.2);

        quoteHub.Add(quotes);

        Quote corrected = quotes[147] with { Close = quotes[147].Close + 2m };
        quotes[147] = corrected;
        quoteHub.Add(corrected);

        observer.Results.IsExactly(quotes.ToParabolicSar(0.02, 0.2));
    }
}
