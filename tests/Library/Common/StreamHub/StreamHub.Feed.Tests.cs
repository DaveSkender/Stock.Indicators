namespace Observables;

[TestClass]
public class Feed : TestBase
{
    public TestContext TestContext { get; set; }

    private static readonly TimeSpan Budget = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Blocks the hub's single writer inside a notification until released.
    /// </summary>
    private sealed class GateObserver : IStreamObserver<IBar>, IDisposable
    {
        private readonly ManualResetEventSlim _entered = new(false);
        private readonly ManualResetEventSlim _release = new(false);

        public bool IsSubscribed => true;
        public void Unsubscribe() { }
        public void OnAdd(IBar item, bool notify, int? indexHint)
        {
            _entered.Set();
            _release.Wait(Budget);
        }

        public void OnRebuild(DateTime fromTimestamp) { }
        public void OnPrune(DateTime toTimestamp) { }
        public void OnError(Exception exception) { }
        public void OnCompleted() { }
        public void WaitEntered() => _entered.Wait(Budget).Should().BeTrue();
        public void Release() => _release.Set();

        public void Dispose()
        {
            _entered.Dispose();
            _release.Dispose();
        }
    }

    [TestMethod]
    public async Task Feed_SingleProducer_AppliesInOrder_AndEndsTransmission()
    {
        IReadOnlyList<Bar> bars = Bars.Take(50).ToList();
        BarHub hub = new();
        SmaHub sma = hub.ToSmaHub(10);

        StreamFeed<IBar> feed = hub.CreateFeed(endTransmissionOnDispose: true);

        try
        {
            foreach (Bar bar in bars)
            {
                await feed.WriteAsync(bar, TestContext.CancellationToken).ConfigureAwait(true);
            }
        }
        finally
        {
            await feed.DisposeAsync().ConfigureAwait(true);
        }

        hub.Cache.Should().HaveCount(50);
        sma.Results.IsExactly(bars.ToSma(10));
        sma.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public async Task Feed_ConcurrentProducers_LoseNothing()
    {
        IReadOnlyList<Bar> bars = Bars.Take(200).ToList();
        BarHub hub = new();

        StreamFeed<IBar> feed = hub.CreateFeed();

        try
        {
            await Task.WhenAll(Enumerable.Range(0, 4).Select(p => Task.Run(async () => {
                for (int i = p; i < bars.Count; i += 4)
                {
                    await feed.WriteAsync(bars[i], TestContext.CancellationToken).ConfigureAwait(true);
                }
            }, TestContext.CancellationToken))).ConfigureAwait(false);
        }
        finally
        {
            await feed.DisposeAsync().ConfigureAwait(true);
        }

        // out-of-order arrival across producers heals to the sorted timeline
        hub.Cache.Should().Equal(bars);
    }

    [TestMethod]
    public async Task Feed_FullQueue_Wait_BlocksWriterUntilDrained()
    {
        IReadOnlyList<Bar> bars = Bars.Take(10).ToList();
        BarHub hub = new();
        using GateObserver gate = new();
        hub.Subscribe(gate);

        StreamFeed<IBar> feed = hub.CreateFeed(capacity: 1);

        try
        {
            await feed.WriteAsync(bars[0], TestContext.CancellationToken).ConfigureAwait(true);
            gate.WaitEntered();          // consumer holds bars[0]
            await feed.WriteAsync(bars[1], TestContext.CancellationToken).ConfigureAwait(true); // fills the queue

            ValueTask blocked = feed.WriteAsync(bars[2], TestContext.CancellationToken);
            blocked.IsCompleted.Should().BeFalse();

            gate.Release();
            await blocked.ConfigureAwait(true);
        }
        finally
        {
            gate.Release();
            await feed.DisposeAsync().ConfigureAwait(true);
        }

        hub.Cache.Should().Equal(bars.Take(3));
    }

    [TestMethod]
    public async Task Feed_FullQueue_DropOldest_KeepsNewest()
    {
        IReadOnlyList<Bar> bars = Bars.Take(10).ToList();
        BarHub hub = new();
        using GateObserver gate = new();
        hub.Subscribe(gate);

        StreamFeed<IBar> feed = hub.CreateFeed(capacity: 2, StreamFeedFullMode.DropOldest);

        await feed.WriteAsync(bars[0], TestContext.CancellationToken).ConfigureAwait(true);
        gate.WaitEntered();
        await feed.WriteAsync(bars[1], TestContext.CancellationToken).ConfigureAwait(true);
        await feed.WriteAsync(bars[2], TestContext.CancellationToken).ConfigureAwait(true);
        await feed.WriteAsync(bars[3], TestContext.CancellationToken).ConfigureAwait(false); // drops bars[1]

        gate.Release();
        await feed.DisposeAsync().ConfigureAwait(true);

        hub.Cache.Select(static b => b.Timestamp).Should().Equal(
            bars[0].Timestamp, bars[2].Timestamp, bars[3].Timestamp);
    }

    [TestMethod]
    public async Task Feed_RejectedItem_SurfacesOnNextWriteAndDispose()
    {
#pragma warning disable CA2000 // disposed below, where the rejection is asserted
        StreamFeed<int> feed = new(
            static i => {
                if (i == 2)
                {
                    throw new InvalidOperationException("rejected");
                }
            },
            capacity: 4,
            StreamFeedFullMode.Wait,
            endTransmission: null);
#pragma warning restore CA2000

        await feed.WriteAsync(1, TestContext.CancellationToken).ConfigureAwait(true);
        await feed.WriteAsync(2, TestContext.CancellationToken).ConfigureAwait(true);

        // the consumer faults asynchronously; a later write reports it
        Func<Task> keepWriting = async () => {
            for (int i = 0; i < 1000; i++)
            {
                await feed.WriteAsync(3, TestContext.CancellationToken).ConfigureAwait(true);
                await Task.Delay(5, TestContext.CancellationToken).ConfigureAwait(true);
            }
        };

        await keepWriting.Should().ThrowAsync<InvalidOperationException>().WithMessage("rejected").ConfigureAwait(true);

        Func<Task> dispose = async () => await feed.DisposeAsync().ConfigureAwait(false);
        await dispose.Should().ThrowAsync<InvalidOperationException>().WithMessage("rejected").ConfigureAwait(true);
    }

    [TestMethod]
    public async Task Feed_WriteAfterDispose_Throws()
    {
        BarHub hub = new();
        StreamFeed<IBar> feed = hub.CreateFeed();
        await feed.DisposeAsync().ConfigureAwait(true);

        Func<Task> write = async () => await feed.WriteAsync(Bars[0], TestContext.CancellationToken).ConfigureAwait(false);

        await write.Should().ThrowAsync<InvalidOperationException>().ConfigureAwait(true);
        await feed.DisposeAsync().ConfigureAwait(false); // second dispose is a no-op
    }

    [TestMethod]
    public void CreateFeed_OnChainedHub_Throws()
    {
        BarHub hub = new();
        SmaHub sma = hub.ToSmaHub(5);

        Action create = () => sma.CreateFeed();

        create.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void CreateFeed_BadCapacity_Throws()
    {
        BarHub hub = new();

        Action create = () => hub.CreateFeed(capacity: 0);

        create.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public async Task Feed_ManyProducers_ApplyOneItemAtATime_InProducerOrder()
    {
        const int producers = 8;
        const int perProducer = 100;

        int active = 0;
        int maxActive = 0;
        List<(int Producer, int Seq)> applied = [];

        StreamFeed<(int Producer, int Seq)> feed = new(
            item => {
                int now = Interlocked.Increment(ref active);
                int seen;
                while (now > (seen = Volatile.Read(ref maxActive)))
                {
                    Interlocked.CompareExchange(ref maxActive, now, seen);
                }

                // hold the call open long enough for a second, unserialized caller to enter
                SpinWait.SpinUntil(() => Volatile.Read(ref active) > 1, 1);
                applied.Add(item);
                Interlocked.Decrement(ref active);
            },
            capacity: 16,
            StreamFeedFullMode.Wait,
            endTransmission: null);

        try
        {
            await Task.WhenAll(Enumerable.Range(0, producers).Select(p => Task.Run(async () => {
                for (int i = 0; i < perProducer; i++)
                {
                    await feed.WriteAsync((p, i), TestContext.CancellationToken).ConfigureAwait(false);
                }
            }, TestContext.CancellationToken))).ConfigureAwait(true);
        }
        finally
        {
            await feed.DisposeAsync().ConfigureAwait(true);
        }

        maxActive.Should().Be(1);
        applied.Should().HaveCount(producers * perProducer);

        for (int p = 0; p < producers; p++)
        {
            applied.Where(a => a.Producer == p).Select(static a => a.Seq)
                .Should().Equal(Enumerable.Range(0, perProducer));
        }
    }

    [TestMethod]
    public async Task Feed_RejectedItem_ReleasesWriterBlockedOnFullQueue()
    {
        using ManualResetEventSlim gate = new(false);
        using ManualResetEventSlim entered = new(false);

#pragma warning disable CA2000 // disposed below, where the rejection is asserted
        StreamFeed<int> feed = new(
            i => {
                if (i == 0)
                {
                    entered.Set();
                    gate.Wait(Budget, TestContext.CancellationToken);
                    throw new OverflowException("rejected");
                }
            },
            capacity: 1,
            StreamFeedFullMode.Wait,
            endTransmission: null);
#pragma warning restore CA2000

        await feed.WriteAsync(0, TestContext.CancellationToken).ConfigureAwait(true);
        entered.Wait(Budget, TestContext.CancellationToken).Should().BeTrue();
        await feed.WriteAsync(1, TestContext.CancellationToken).ConfigureAwait(true); // fills the queue

        ValueTask blocked = feed.WriteAsync(2, TestContext.CancellationToken);
        blocked.IsCompleted.Should().BeFalse();

        gate.Set();

        Func<Task> wait = async () => await blocked.AsTask().WaitAsync(Budget, TestContext.CancellationToken)
            .ConfigureAwait(true);

        // the blocked writer gets the hub's own exception, not a hang or a generic closed error
        await wait.Should().ThrowAsync<OverflowException>().ConfigureAwait(true);

        Func<Task> completion = () => feed.Completion;
        await completion.Should().ThrowAsync<OverflowException>().ConfigureAwait(true);

        Func<Task> dispose = async () => await feed.DisposeAsync().ConfigureAwait(true);
        await dispose.Should().ThrowAsync<OverflowException>().ConfigureAwait(true);
    }

    [TestMethod]
    public async Task Feed_WriteCanceledWhileWaitingForSpace_Throws()
    {
        using GateObserver gate = new();
        BarHub hub = new();
        hub.Subscribe(gate);

        StreamFeed<IBar> feed = hub.CreateFeed(capacity: 1);

        try
        {
            await feed.WriteAsync(Bars[0], TestContext.CancellationToken).ConfigureAwait(true);
            gate.WaitEntered();
            await feed.WriteAsync(Bars[1], TestContext.CancellationToken).ConfigureAwait(true);

            using CancellationTokenSource cts = new();
            ValueTask blocked = feed.WriteAsync(Bars[2], cts.Token);
            blocked.IsCompleted.Should().BeFalse();

            await cts.CancelAsync().ConfigureAwait(true);

            Func<Task> wait = async () => await blocked.AsTask().ConfigureAwait(true);
            await wait.Should().ThrowAsync<OperationCanceledException>().ConfigureAwait(true);
        }
        finally
        {
            gate.Release();
            await feed.DisposeAsync().ConfigureAwait(true);
        }
    }

    [TestMethod]
    public async Task Feed_DefaultDispose_LeavesChainedHubsSubscribed()
    {
        IReadOnlyList<Bar> bars = Bars.Take(50).ToList();
        BarHub hub = new();
        SmaHub sma = hub.ToSmaHub(10);

        StreamFeed<IBar> feed = hub.CreateFeed();

        foreach (Bar bar in bars)
        {
            await feed.WriteAsync(bar, TestContext.CancellationToken).ConfigureAwait(true);
        }

        await feed.DisposeAsync().ConfigureAwait(true);

        sma.IsSubscribed.Should().BeTrue("ending the transmission is opt-in");
        sma.Results.IsExactly(bars.ToSma(10));
    }

    [TestMethod]
    public async Task CreateFeed_WhileAnotherIsOpen_Throws_UntilItIsDisposed()
    {
        BarHub hub = new();
        StreamFeed<IBar> first = hub.CreateFeed();

        Action second = () => hub.CreateFeed();
        second.Should().Throw<InvalidOperationException>().WithMessage("*still open*");

        await first.DisposeAsync().ConfigureAwait(true);

        StreamFeed<IBar> next = hub.CreateFeed();
        await next.DisposeAsync().ConfigureAwait(true);
    }
}
