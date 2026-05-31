namespace Observables;

/// <summary>
/// Pins the <c>Snapshot()</c> contract: an immutable point-in-time copy of the
/// cache that is decoupled from the live <c>Results</c> view, so it can be
/// safely handed to another thread.
/// </summary>
[TestClass]
public class Snapshot : TestBase
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void Snapshot_MatchesResultsContent()
    {
        QuoteHub quoteHub = new();
        SmaHub smaHub = quoteHub.ToSmaHub(20);

        quoteHub.Add(Quotes.Take(50));

        IReadOnlyList<SmaResult> snapshot = smaHub.Snapshot();

        snapshot.Should().HaveCount(smaHub.Results.Count);
        snapshot.Should().Equal(smaHub.Results);

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Snapshot_IsDecoupledFromLiveCache()
    {
        QuoteHub quoteHub = new();

        quoteHub.Add(Quotes.Take(30));

        IReadOnlyList<IQuote> snapshot = quoteHub.Snapshot();
        snapshot.Should().HaveCount(30);

        // mutate the hub after taking the snapshot
        quoteHub.Add(Quotes.Skip(30).Take(20));
        quoteHub.RemoveAt(0);

        // the snapshot is a detached copy — unchanged by later mutation
        snapshot.Should().HaveCount(30);

        // ...while the live view reflects the changes
        quoteHub.Results.Should().HaveCount(49);

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Snapshot_IsNotTheLiveResultsInstance()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(10));

        IReadOnlyList<IQuote> snapshot = quoteHub.Snapshot();

        snapshot.Should().NotBeSameAs(quoteHub.Results);
        snapshot.Should().Equal(quoteHub.Results);

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public async Task Snapshot_ConcurrentWithAdds_NeverThrowsAndStaysConsistent()
    {
        // Snapshot() copies under the cache lock, so a reader taking snapshots
        // while a single writer adds must never throw and each snapshot must be
        // an internally consistent, sorted prefix.
        QuoteHub quoteHub = new();
        List<Quote> quotes = Quotes.Take(400).ToList();

        using CancellationTokenSource cts = new();

        Task reader = Task.Run(() => {
            while (!cts.IsCancellationRequested)
            {
                IReadOnlyList<IQuote> snap = quoteHub.Snapshot();

                // a snapshot is always sorted ascending (internally consistent)
                for (int i = 1; i < snap.Count; i++)
                {
                    snap[i].Timestamp.Should().BeOnOrAfter(snap[i - 1].Timestamp);
                }
            }
        }, TestContext.CancellationToken);

        // single writer; cancel the reader when done (even if the writer faults,
        // so WhenAll surfaces that fault promptly instead of hanging to timeout)
        Task writer = Task.Run(() => {
            try
            {
                foreach (Quote q in quotes)
                {
                    quoteHub.Add(q);
                }
            }
            finally
            {
                cts.Cancel();
            }
        }, TestContext.CancellationToken);

        await Task.WhenAll(reader, writer).ConfigureAwait(false); // surfaces any assertion failure

        quoteHub.Snapshot().Should().HaveCount(quotes.Count);

        quoteHub.EndTransmission();
    }
}
