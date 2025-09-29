namespace StreamHub;

[TestClass]
public class MamaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        MamaHub<Quote> observer = provider
            .ToMama(fastLimit, slowLimit);

        // fetch initial results (early)
        IReadOnlyList<MamaResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
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
        provider.Insert(quotesList[80]);

        // get actual results
        IReadOnlyList<MamaResult> actualList
            = observer.Results;

        // trim to exclude extra quotes that may be added
        IReadOnlyList<MamaResult> actual
            = actualList.Take(502).ToList();

        // get expected results
        IReadOnlyList<MamaResult> expected
            = Quotes.ToMama(fastLimit, slowLimit);

        // assertions
        Assert.HasCount(502, actual);
        Assert.HasCount(502, expected);

        for (int i = 0; i < 502; i++)
        {
            MamaResult a = actual[i];
            MamaResult e = expected[i];

            // compare results
            Assert.AreEqual(e.Timestamp, a.Timestamp);

            if (e.Mama is null)
            {
                Assert.IsNull(a.Mama);
            }
            else
            {
                Assert.AreEqual(e.Mama.Round(4), a.Mama.Round(4));
            }

            if (e.Fama is null)
            {
                Assert.IsNull(a.Fama);
            }
            else
            {
                Assert.AreEqual(e.Fama.Round(4), a.Fama.Round(4));
            }
        }
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        MamaHub<Quote> observer = provider.ToMama(fastLimit, slowLimit);

        Assert.AreEqual($"MAMA({fastLimit},{slowLimit})", observer.ToString());
    }

    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize chain
        SmaHub<Quote> sma = provider.ToSma(10);
        MamaHub<SmaResult> observer = sma.ToMama(fastLimit, slowLimit);

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // get actual results
        IReadOnlyList<MamaResult> actualList = observer.Results;
        IReadOnlyList<MamaResult> actual = actualList.Take(502).ToList();

        // get expected results
        IReadOnlyList<MamaResult> expected = Quotes
            .ToSma(10)
            .ToMama(fastLimit, slowLimit);

        // assertions
        Assert.HasCount(502, actual);
        Assert.HasCount(502, expected);

        for (int i = 0; i < 502; i++)
        {
            MamaResult a = actual[i];
            MamaResult e = expected[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);

            if (e.Mama is null)
            {
                Assert.IsNull(a.Mama);
            }
            else
            {
                Assert.AreEqual(e.Mama.Round(4), a.Mama.Round(4));
            }

            if (e.Fama is null)
            {
                Assert.IsNull(a.Fama);
            }
            else
            {
                Assert.AreEqual(e.Fama.Round(4), a.Fama.Round(4));
            }
        }
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize chain
        MamaHub<Quote> mama = provider.ToMama(fastLimit, slowLimit);
        SmaHub<MamaResult> observer = mama.ToSma(10);

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // get actual results
        IReadOnlyList<SmaResult> actualList = observer.Results;
        IReadOnlyList<SmaResult> actual = actualList.Take(502).ToList();

        // get expected results
        IReadOnlyList<SmaResult> expected = Quotes
            .ToMama(fastLimit, slowLimit)
            .ToSma(10);

        // assertions
        Assert.HasCount(502, actual);
        Assert.HasCount(502, expected);

        for (int i = 0; i < 502; i++)
        {
            SmaResult a = actual[i];
            SmaResult e = expected[i];

            Assert.AreEqual(e.Timestamp, a.Timestamp);

            if (e.Sma is null)
            {
                Assert.IsNull(a.Sma);
            }
            else
            {
                Assert.AreEqual(e.Sma.Round(4), a.Sma.Round(4));
            }
        }
    }
}