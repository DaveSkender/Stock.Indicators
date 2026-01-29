namespace StreamHubs;

[TestClass]
public class TickStreamHubTests : StreamHubTestBase
{
    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        TickHub hub = new();
        string result = hub.ToString();

        result.Should().Contain("TICKS");
        result.Should().Contain("0 items");

        hub.EndTransmission();
    }

    [TestMethod]
    public void StandaloneInitialization_WorksCorrectly()
    {
        TickHub hub = new();

        hub.Should().NotBeNull();
        hub.MaxCacheSize.Should().BeGreaterThan(0);
        hub.Cache.Should().BeEmpty();

        hub.EndTransmission();
    }

    [TestMethod]
    public void ProviderBackedInitialization_WorksCorrectly()
    {
        TickHub provider = new();
        TickHub hub = new(provider);

        hub.Should().NotBeNull();
        hub.Cache.Should().BeEmpty();

        hub.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void SameTimestamp_WithoutExecutionId_ReplacesAndNotifiesAsAddition()
    {
        TickHub hub = new();
        int addCount = 0;

        TestTickObserver observer = new();
        observer.OnAddAction = (tick, notify, idx) => addCount++;

        hub.Subscribe(observer);

        // Add first tick without execution ID
        Tick tick1 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, null);
        hub.Add(tick1);

        // Add second tick with same timestamp but different price (no execution ID)
        Tick tick2 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            105m, 15m, null);
        hub.Add(tick2);

        // Cache should have only one tick (the latest)
        hub.Cache.Should().HaveCount(1);
        hub.Cache[0].Price.Should().Be(105m);
        hub.Cache[0].Volume.Should().Be(15m);

        // Both ticks should be notified as additions for aggregators to process
        addCount.Should().Be(2);

        observer.Unsubscribe();
        hub.EndTransmission();
    }

    [TestMethod]
    public void SameTimestamp_WithSameExecutionId_ReplacesAndNotifiesRebuild()
    {
        TickHub hub = new();
        int rebuildCount = 0;

        TestTickObserver observer = new();
        observer.OnRebuildAction = (ts) => rebuildCount++;

        hub.Subscribe(observer);

        // Add first tick with execution ID
        Tick tick1 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, "EXEC-001");
        hub.Add(tick1);

        // Add updated tick with same execution ID (correction)
        Tick tick2 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            101m, 11m, "EXEC-001");
        hub.Add(tick2);

        // Cache should have only one tick (the corrected one)
        hub.Cache.Should().HaveCount(1);
        hub.Cache[0].Price.Should().Be(101m);
        hub.Cache[0].ExecutionId.Should().Be("EXEC-001");

        // Should trigger rebuild for correction
        rebuildCount.Should().Be(1);

        observer.Unsubscribe();
        hub.EndTransmission();
    }

    [TestMethod]
    public void SameTimestamp_WithDifferentExecutionId_NotifiesAddition()
    {
        TickHub hub = new();
        int addCount = 0;

        TestTickObserver observer = new();
        observer.OnAddAction = (tick, notify, idx) => addCount++;

        hub.Subscribe(observer);

        // Add first tick with execution ID
        Tick tick1 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, "EXEC-001");
        hub.Add(tick1);

        // Add second tick with same timestamp but different execution ID
        Tick tick2 = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            105m, 15m, "EXEC-002");
        hub.Add(tick2);

        // Both ticks should be notified to observers (not dropped)
        addCount.Should().Be(2);

        observer.Unsubscribe();
        hub.EndTransmission();
    }

    [TestMethod]
    public void ExactDuplicate_DefersToAppendCache()
    {
        TickHub hub = new();
        int addCount = 0;

        TestTickObserver observer = new();
        observer.OnAddAction = (tick, notify, idx) => addCount++;

        hub.Subscribe(observer);

        // Add same tick twice
        Tick tick = new(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, "EXEC-001");

        hub.Add(tick);
        hub.Add(tick);

        // Should use overflow tracking from base class
        hub.Cache.Should().HaveCount(1);
        addCount.Should().Be(1); // Only first add notified

        observer.Unsubscribe();
        hub.EndTransmission();
    }

    [TestMethod]
    public void StandaloneRebuild_PreservesCacheAndNotifiesObservers()
    {
        TickHub hub = new();
        int rebuildCount = 0;
        DateTime? rebuildTimestamp = null;

        TestTickObserver observer = new();
        observer.OnRebuildAction = (ts) =>
        {
            rebuildCount++;
            rebuildTimestamp = ts;
        };

        hub.Subscribe(observer);

        // Add ticks
        hub.Add(new Tick(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100m, 10m));
        hub.Add(new Tick(DateTime.Parse("2023-11-09 10:01", invariantCulture), 101m, 11m));
        hub.Add(new Tick(DateTime.Parse("2023-11-09 10:02", invariantCulture), 102m, 12m));

        int initialCount = hub.Cache.Count;

        // Trigger rebuild
        hub.Rebuild(DateTime.Parse("2023-11-09 10:01", invariantCulture));

        // Cache should still have all ticks (standalone doesn't clear)
        hub.Cache.Should().HaveCount(initialCount);

        // Observers should be notified
        rebuildCount.Should().Be(1);
        rebuildTimestamp.Should().Be(DateTime.Parse("2023-11-09 10:01", invariantCulture));

        observer.Unsubscribe();
        hub.EndTransmission();
    }

    [TestMethod]
    public void ProviderBackedRebuild_UsesStandardBehavior()
    {
        TickHub provider = new();
        TickHub hub = new(provider);

        // Add ticks through provider
        provider.Add(new Tick(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100m, 10m));
        provider.Add(new Tick(DateTime.Parse("2023-11-09 10:01", invariantCulture), 101m, 11m));
        provider.Add(new Tick(DateTime.Parse("2023-11-09 10:02", invariantCulture), 102m, 12m));

        hub.Cache.Should().HaveCount(3);

        // Trigger rebuild on hub (not provider)
        hub.Rebuild(DateTime.Parse("2023-11-09 10:01", invariantCulture));

        // Provider-backed hub should rebuild normally
        hub.Cache.Should().HaveCountGreaterOrEqualTo(2);

        hub.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void MultipleTicksSameTimestamp_AllProcessedByAggregators()
    {
        // This test verifies the fix for issue #4 from code review
        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Add three ticks at same timestamp with different execution IDs
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, "EXEC-001"));
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            105m, 15m, "EXEC-002"));
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            95m, 20m, "EXEC-003"));

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        // All three ticks should be incorporated into the bar
        IQuote bar = results[0];
        bar.High.Should().Be(105m); // Max of all three
        bar.Low.Should().Be(95m);   // Min of all three
        bar.Volume.Should().Be(45m); // Sum of all three (10 + 15 + 20)

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    /// <summary>
    /// Test observer helper class for tracking tick notifications.
    /// </summary>
    private class TestTickObserver : IStreamObserver<ITick>
    {
        public Action<ITick, bool, int?>? OnAddAction { get; set; }
        public Action<DateTime>? OnRebuildAction { get; set; }
        public bool IsSubscribed { get; private set; } = true;

        public void OnAdd(ITick item, bool notify, int? indexHint)
            => OnAddAction?.Invoke(item, notify, indexHint);

        public void OnCompleted() => IsSubscribed = false;

        public void OnRebuild(DateTime timestamp)
            => OnRebuildAction?.Invoke(timestamp);

        public void OnError(Exception error) { }

        public void OnPrune(DateTime timestamp) { }

        public void Unsubscribe() => IsSubscribed = false;
    }
}
