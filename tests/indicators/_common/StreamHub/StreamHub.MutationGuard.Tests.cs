namespace Observables;

/// <summary>
/// Pins the root-hub mutation contract: only a root hub (a standalone
/// <see cref="QuoteHub"/> or <see cref="TickHub"/>) may have its input timeline
/// mutated directly. A subscribed (chained) hub is driven by its provider, so
/// the public mutating surface — Add, RemoveAt, RemoveRange, Reinitialize, and
/// the Remove(IQuote/ITick) convenience — is rejected on it. Also covers the
/// Remove-by-timestamp convenience on the root hub.
/// </summary>
[TestClass]
public class MutationGuard : TestBase
{
    // ROOT HUB — mutation allowed

    [TestMethod]
    public void RootHub_AllowsMutation()
    {
        QuoteHub quoteHub = new();

        // single + batch add
        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes.Skip(1).Take(49));
        quoteHub.Quotes.Should().HaveCount(50);

        // remove by index
        quoteHub.RemoveAt(49);
        quoteHub.Quotes.Should().HaveCount(49);

        // remove range by timestamp
        quoteHub.RemoveRange(Quotes[40].Timestamp, notify: true);
        quoteHub.Quotes.Should().HaveCount(40);

        // remove range by index
        quoteHub.RemoveRange(30, notify: true);
        quoteHub.Quotes.Should().HaveCount(30);

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ConstructionTimeReinitialize_IsAllowed()
    {
        // every indicator hub calls Reinitialize() from its constructor; the
        // guard must not break that. Build a chained hub and confirm it warms
        // up correctly (proving the construction-time Reinitialize ran).
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);

        quoteHub.Add(Quotes.Take(50));

        observer.Results.Should().HaveCount(50);
        observer.Results.IsExactly(Quotes.Take(50).ToList().ToSma(20));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    // SUBSCRIBED HUB — mutation rejected

    [TestMethod]
    public void SubscribedHub_Add_Throws()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.Add(Quotes[50]));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.Add(Quotes.Skip(50).Take(5)));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SubscribedHub_RemoveAt_Throws()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.RemoveAt(30));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SubscribedHub_RemoveRange_Throws()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.RemoveRange(Quotes[30].Timestamp, notify: true));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.RemoveRange(30, notify: true));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SubscribedHub_Reinitialize_Throws()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        Assert.ThrowsExactly<InvalidOperationException>(observer.Reinitialize);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SubscribedHub_StaysInSyncDespiteGuard()
    {
        // the guard must not interfere with normal provider-driven operation:
        // late arrivals and root-hub removals still cascade to the observer.
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);

        List<Quote> quotes = Quotes.Take(200).ToList();

        // feed all but one, then add the skipped quote late
        for (int i = 0; i < quotes.Count; i++)
        {
            if (i == 80) { continue; }
            quoteHub.Add(quotes[i]);
        }

        quoteHub.Add(quotes[80]);     // late arrival → rollback + replay

        const int removeIndex = 120;
        quoteHub.RemoveAt(removeIndex);  // remove from root → cascade

        List<Quote> expected = [.. quotes];
        expected.RemoveAt(removeIndex);
        observer.Results.IsExactly(expected.ToSma(20));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    // Remove(IQuote) convenience — root QuoteHub

    [TestMethod]
    public void RemoveByQuote_RemovesByTimestamp_CascadesToObserver()
    {
        QuoteHub quoteHub = new();
        SmaHub observer = quoteHub.ToSmaHub(20);

        List<Quote> quotes = Quotes.Take(50).ToList();
        quoteHub.Add(quotes);

        // a proxy quote at the same timestamp (different values) must still
        // match — Remove locates the entry by timestamp, not by equality
        Quote proxy = quotes[30] with { Close = quotes[30].Close + 5m };
        quoteHub.Remove(proxy);

        quoteHub.Quotes.Should().HaveCount(49);

        List<Quote> expected = [.. quotes];
        expected.RemoveAt(30);
        observer.Results.IsExactly(expected.ToSma(20));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void RemoveByQuote_AbsentTimestamp_Throws()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        // a timestamp not present in the cache
        Quote absent = Quotes[0] with { Timestamp = Quotes[0].Timestamp.AddTicks(-1) };

        Assert.ThrowsExactly<ArgumentException>(() => quoteHub.Remove(absent));

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void RemoveByQuote_Null_Throws()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(10));

        Assert.ThrowsExactly<ArgumentNullException>(() => quoteHub.Remove(null!));

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void RemoveByQuote_OnSubscribedHub_Throws()
    {
        QuoteHub provider = new();
        QuoteHub observer = provider.ToQuoteHub();
        provider.Add(Quotes.Take(50));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => observer.Remove(Quotes[30]));

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    // Remove(ITick) convenience — root TickHub

    [TestMethod]
    public void RemoveByTick_RemovesByTimestamp()
    {
        DateTime start = new(2023, 11, 9, 10, 0, 0, DateTimeKind.Utc);

        List<Tick> ticks = [];
        for (int i = 0; i < 30; i++)
        {
            ticks.Add(new Tick(start.AddMinutes(i), 100m + i, 10m + i));
        }

        TickHub tickHub = new();
        tickHub.Add(ticks);
        tickHub.Cache.Should().HaveCount(30);

        tickHub.Remove(ticks[15]);
        tickHub.Cache.Should().HaveCount(29);
        tickHub.Cache.Any(t => t.Timestamp == ticks[15].Timestamp).Should().BeFalse();

        // absent timestamp throws
        Tick absent = new(start.AddMinutes(-1), 1m, 1m);
        Assert.ThrowsExactly<ArgumentException>(() => tickHub.Remove(absent));

        tickHub.EndTransmission();
    }
}
