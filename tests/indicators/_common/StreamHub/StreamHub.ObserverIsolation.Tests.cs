namespace Observables;

/// <summary>
/// Pins observer isolation: a subscriber that throws from a notification
/// callback must not abort the hub's fan-out to its sibling subscribers, and
/// must not surface as an exception out of <c>Add</c>. The faulting observer's
/// exception is routed to its own <c>OnError</c>.
/// </summary>
[TestClass]
public class ObserverIsolation : TestBase
{
    private sealed class ThrowingObserver : IStreamObserver<IQuote>
    {
        public int OnErrorCount { get; private set; }
        public bool IsSubscribed { get; private set; } = true;
        public void Unsubscribe() => IsSubscribed = false;
        public void OnAdd(IQuote item, bool notify, int? indexHint)
            => throw new InvalidOperationException("observer boom (OnAdd)");
        public void OnRebuild(DateTime fromTimestamp)
            => throw new InvalidOperationException("observer boom (OnRebuild)");
        public void OnPrune(DateTime toTimestamp)
            => throw new InvalidOperationException("observer boom (OnPrune)");
        public void OnError(Exception exception) => OnErrorCount++;
        public void OnCompleted() { }
    }

    private sealed class RecordingObserver : IStreamObserver<IQuote>
    {
        public int OnAddCount { get; private set; }
        public int OnRebuildCount { get; private set; }
        public bool IsSubscribed { get; private set; } = true;
        public void Unsubscribe() => IsSubscribed = false;
        public void OnAdd(IQuote item, bool notify, int? indexHint) => OnAddCount++;
        public void OnRebuild(DateTime fromTimestamp) => OnRebuildCount++;
        public void OnPrune(DateTime toTimestamp) { }
        public void OnError(Exception exception) { }
        public void OnCompleted() { }
    }

    [TestMethod]
    public void ThrowingObserver_OnAdd_IsIsolatedFromHubAndSiblings()
    {
        IReadOnlyList<Quote> quotes = Quotes.Take(20).ToList();

        QuoteHub hub = new();
        ThrowingObserver thrower = new();
        RecordingObserver recorder = new();
        hub.Subscribe(thrower);
        hub.Subscribe(recorder);

        // Without isolation, the thrower's OnAdd exception propagates out of Add.
        Action feed = () => {
            foreach (Quote q in quotes)
            {
                hub.Add(q);
            }
        };

        feed.Should().NotThrow("a faulting observer must be isolated from the hub");
        recorder.OnAddCount.Should().Be(quotes.Count, "the sibling observer must receive every update");
        thrower.OnErrorCount.Should().Be(quotes.Count, "the faulting observer's OnError is called once per failed OnAdd");

        hub.EndTransmission();
    }

    [TestMethod]
    public void ThrowingObserver_OnRebuild_IsIsolatedFromSiblings()
    {
        // A late arrival triggers the OnRebuild fan-out; a thrower there must not
        // starve siblings of the rebuild notification either.
        IReadOnlyList<Quote> quotes = Quotes.Take(60).ToList();

        QuoteHub hub = new();
        ThrowingObserver thrower = new();
        RecordingObserver recorder = new();
        hub.Subscribe(thrower);
        hub.Subscribe(recorder);

        Action feed = () => {
            for (int i = 0; i < quotes.Count; i++)
            {
                if (i == 30) { continue; }
                hub.Add(quotes[i]);
            }

            hub.Add(quotes[30]); // late arrival -> rebuild fan-out
        };

        feed.Should().NotThrow("a faulting observer must be isolated during rebuild fan-out");
        recorder.OnRebuildCount.Should().Be(1, "the single late arrival triggers exactly one rebuild fan-out");

        hub.EndTransmission();
    }
}
