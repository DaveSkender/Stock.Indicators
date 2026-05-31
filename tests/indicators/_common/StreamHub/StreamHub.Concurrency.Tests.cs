using System.Collections.Concurrent;

namespace Observables;

/// <summary>
/// Safety net for the documented streaming threading contract: a single writer
/// (one thread, or many funneled through a gate) produces results identical to
/// the batch Series, and concurrent readers / observer churn never corrupt the
/// hub. These are end-to-end concurrency tests through real indicator hubs,
/// complementing the low-level race-guard pins.
/// </summary>
[TestClass]
public class Concurrency : TestBase
{
    [TestMethod]
    public void SingleWriterGate_OutOfOrderParallelProducers_ConvergeToSeries()
    {
        // N producers each feed a strided subset (ascending), funneled through a
        // single-writer gate. Threads interleave, so quotes arrive globally
        // out of order and drive rollback/replay. Regardless of arrival order,
        // every quote is added exactly once under serialization, so a deep chain
        // must converge bit-for-bit to the batch Series.
        const int count = 500;
        const int producerCount = 8;
        const int emaPeriods = 20;
        const int smaPeriods = 10;

        List<Quote> quotes = Quotes.Take(count).ToList();
        IReadOnlyList<EmaResult> expectedEma = quotes.ToEma(emaPeriods);
        IReadOnlyList<SmaResult> expectedSma = expectedEma.ToSma(smaPeriods);

        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(emaPeriods);
        SmaHub smaHub = emaHub.ToSmaHub(smaPeriods);

        // Seed the global-minimum quote first so no later out-of-order arrival
        // is "before head". A standalone QuoteHub drops before-head quotes by
        // design; once index 0 is the head, every other quote is >= it and
        // lands mid-cache (rollback) or at the tail.
        quoteHub.Add(quotes[0]);

        object gate = new();
        Task[] producers = new Task[producerCount];
        for (int t = 0; t < producerCount; t++)
        {
            int start = t;
            producers[t] = Task.Run(() => {
                for (int i = start; i < count; i += producerCount)
                {
                    if (i == 0) { continue; }  // already seeded
                    lock (gate)
                    {
                        quoteHub.Add(quotes[i]);
                    }
                }
            });
        }

        Task.WaitAll(producers);

        quoteHub.Quotes.Should().HaveCount(count);
        emaHub.Results.IsExactly(expectedEma);
        smaHub.Results.IsExactly(expectedSma);

        smaHub.Unsubscribe();
        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1031:Do not catch general exception types",
        Justification = "The test deliberately records any exception thrown by the concurrent threads to assert thread-safety.")]
    public void ConcurrentSnapshotReaders_DuringFeed_StayConsistentAndConverge()
    {
        // A single writer feeds the chain; reader threads continuously Snapshot
        // a downstream hub. Each snapshot must be internally consistent (sorted)
        // and no read may throw; the final results must equal the Series.
        const int count = 800;
        const int emaPeriods = 20;
        const int readerCount = 4;

        List<Quote> quotes = Quotes.Take(count).ToList();
        IReadOnlyList<EmaResult> expected = quotes.ToEma(emaPeriods);

        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(emaPeriods);

        ConcurrentBag<Exception> failures = [];
        using CancellationTokenSource done = new();
        using ManualResetEventSlim readersStarted = new(false);

        Task[] readers = new Task[readerCount];
        for (int r = 0; r < readerCount; r++)
        {
            readers[r] = Task.Run(() => {
                try
                {
                    while (!done.Token.IsCancellationRequested)
                    {
                        readersStarted.Set();  // rendezvous: guarantee overlap with the feed
                        IReadOnlyList<EmaResult> snap = emaHub.Snapshot();
                        for (int i = 1; i < snap.Count; i++)
                        {
                            snap[i].Timestamp.Should().BeOnOrAfter(snap[i - 1].Timestamp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(ex);
                }
            });
        }

        // ensure at least one reader is live before feeding
        readersStarted.Wait(TimeSpan.FromSeconds(5));

        foreach (Quote q in quotes)
        {
            quoteHub.Add(q);
        }

        done.Cancel();
        Task.WaitAll(readers);

        failures.Should().BeEmpty("concurrent Snapshot() reads must never throw");
        emaHub.Results.IsExactly(expected);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1031:Do not catch general exception types",
        Justification = "The test deliberately records any exception thrown by the concurrent threads to assert thread-safety.")]
    public void SubscribeDisposeChurn_DuringFeed_PreservesStableObserver()
    {
        // A stable downstream hub stays subscribed while another thread churns
        // subscribe/dispose of observers. The stable observer must still match
        // the Series and nothing may throw.
        const int count = 600;
        const int emaPeriods = 20;

        List<Quote> quotes = Quotes.Take(count).ToList();
        IReadOnlyList<EmaResult> expected = quotes.ToEma(emaPeriods);

        QuoteHub quoteHub = new();
        EmaHub stable = quoteHub.ToEmaHub(emaPeriods);

        ConcurrentBag<Exception> failures = [];
        using CancellationTokenSource done = new();
        using ManualResetEventSlim churnStarted = new(false);

        Task churn = Task.Run(() => {
            try
            {
                CountingObserver[] pool = [.. Enumerable.Range(0, 8).Select(_ => new CountingObserver())];
                while (!done.Token.IsCancellationRequested)
                {
                    List<IDisposable> subs = [.. pool.Select(quoteHub.Subscribe)];
                    churnStarted.Set();  // rendezvous: guarantee overlap with the feed
                    foreach (IDisposable s in subs)
                    {
                        s.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                failures.Add(ex);
            }
        });

        // ensure the churn is live before feeding
        churnStarted.Wait(TimeSpan.FromSeconds(5));

        try
        {
            foreach (Quote q in quotes)
            {
                quoteHub.Add(q);
            }
        }
        catch (Exception ex)
        {
            failures.Add(ex);
        }
        finally
        {
            done.Cancel();
            churn.GetAwaiter().GetResult();
        }

        failures.Should().BeEmpty();
        stable.Results.IsExactly(expected);

        stable.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SingleWriterGate_WithLateArrivalAndRemoval_ConvergeToRevisedSeries()
    {
        // Parallel producers (gated) plus a late arrival and a removal, all
        // serialized — the chain converges to the revised Series.
        const int count = 400;
        const int skipIndex = 200;
        const int removeIndex = 120;
        const int emaPeriods = 20;

        List<Quote> quotes = Quotes.Take(count).ToList();

        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(emaPeriods);

        // seed the global-minimum quote first (see the convergence test for why)
        quoteHub.Add(quotes[0]);

        object gate = new();

        // feed everything except one index, from 4 strided producers
        Task[] producers = new Task[4];
        for (int t = 0; t < 4; t++)
        {
            int start = t;
            producers[t] = Task.Run(() => {
                for (int i = start; i < count; i += 4)
                {
                    if (i == 0 || i == skipIndex) { continue; }
                    lock (gate)
                    {
                        quoteHub.Add(quotes[i]);
                    }
                }
            });
        }

        Task.WaitAll(producers);

        // late arrival + removal (single writer)
        lock (gate)
        {
            quoteHub.Add(quotes[skipIndex]);
            quoteHub.RemoveAt(removeIndex);
        }

        List<Quote> revised = [.. quotes];
        revised.RemoveAt(removeIndex);

        emaHub.Results.IsExactly(revised.ToEma(emaPeriods));

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    private sealed class CountingObserver : IStreamObserver<IQuote>
    {
        public bool IsSubscribed => false;
        public void Unsubscribe() { }
        public void OnAdd(IQuote item, bool notify, int? indexHint) { }
        public void OnRebuild(DateTime fromTimestamp) { }
        public void OnPrune(DateTime toTimestamp) { }
        public void OnError(Exception exception) { }
        public void OnCompleted() { }
    }
}
