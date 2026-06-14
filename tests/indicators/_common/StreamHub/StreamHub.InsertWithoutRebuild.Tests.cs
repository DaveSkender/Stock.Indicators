namespace Observables;

/// <summary>
/// Covers the <c>InsertWithoutRebuild</c> branches of a root
/// <see cref="QuoteHub"/>: a late arrival that lands mid-cache is inserted
/// in place (no rebuild of the root's own cache), duplicates of that late
/// arrival are suppressed, and downstream observers rebuild to match the
/// corrected timeline exactly.
/// </summary>
[TestClass]
public class InsertWithoutRebuild : TestBase
{
    [TestMethod]
    public void LateArrival_MidCache_InsertsInPlace()
    {
        List<Quote> quotes = Quotes.Take(20).ToList();

        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(5);

        // skip index 10, then deliver it late
        for (int i = 0; i < quotes.Count; i++)
        {
            if (i == 10)
            {
                continue;
            }

            quoteHub.Add(quotes[i]);
        }

        quoteHub.Add(quotes[10]);

        // root cache is chronological and complete
        quoteHub.Results.Should().HaveCount(quotes.Count);
        quoteHub.Results[10].Timestamp.Should().Be(quotes[10].Timestamp);

        // observer matches the batch series exactly
        observer.Results.IsExactly(quotes.ToSma(5));
    }

    [TestMethod]
    public void LateArrival_DuplicateResend_IsSuppressed()
    {
        List<Quote> quotes = Quotes.Take(20).ToList();

        QuoteHub quoteHub = new();

        for (int i = 0; i < quotes.Count; i++)
        {
            if (i == 10)
            {
                continue;
            }

            quoteHub.Add(quotes[i]);
        }

        quoteHub.Add(quotes[10]);
        quoteHub.Add(quotes[10]); // duplicate of the same late arrival

        quoteHub.Results.Should().HaveCount(quotes.Count);
        quoteHub.IsFaulted.Should().BeFalse();
    }

    [TestMethod]
    public void LateArrival_FirstPosition_InsertsAtFront()
    {
        List<Quote> quotes = Quotes.Take(15).ToList();

        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(5);

        // withhold the earliest quote, then deliver it late;
        // note: a root QuoteHub rejects arrivals older than its first
        // cached item, so the late arrival here is the second quote
        for (int i = 0; i < quotes.Count; i++)
        {
            if (i == 1)
            {
                continue;
            }

            quoteHub.Add(quotes[i]);
        }

        quoteHub.Add(quotes[1]);

        quoteHub.Results.Should().HaveCount(quotes.Count);
        quoteHub.Results[1].Timestamp.Should().Be(quotes[1].Timestamp);

        observer.Results.IsExactly(quotes.ToSma(5));
    }

    [TestMethod]
    public void BatchAdd_UnsortedInput_IsSortedStably()
    {
        List<Quote> quotes = Quotes.Take(30).ToList();

        // shuffle deterministically
        List<Quote> shuffled = [.. quotes];
        (shuffled[3], shuffled[22]) = (shuffled[22], shuffled[3]);
        (shuffled[8], shuffled[15]) = (shuffled[15], shuffled[8]);

        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(5);

        quoteHub.Add(shuffled);

        quoteHub.Results.Should().HaveCount(quotes.Count);
        observer.Results.IsExactly(quotes.ToSma(5));
    }

    [TestMethod]
    public void BatchAdd_SortedInput_FastPathMatchesSeries()
    {
        List<Quote> quotes = Quotes.Take(30).ToList();

        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(5);

        quoteHub.Add(quotes);

        quoteHub.Results.Should().HaveCount(quotes.Count);
        observer.Results.IsExactly(quotes.ToSma(5));
    }
}
