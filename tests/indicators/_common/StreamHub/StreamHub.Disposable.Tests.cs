namespace Observables;

/// <summary>
/// Pins the StreamHub teardown contract: <see cref="IDisposable"/> on
/// every hub, the <c>DisposeChain</c> whole-chain helper, and the
/// <c>EndTransmission → OnCompleted</c> sequence. Disposing a hub detaches it
/// from its provider and completes its own subscribers; <c>DisposeChain</c>
/// does the same depth-first across the entire downstream chain.
/// </summary>
[TestClass]
public class Disposable : TestBase
{
    [TestMethod]
    public void Dispose_DetachesFromProviderAndCompletesSubscribers()
    {
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        SmaHub smaHub = emaHub.ToSmaHub(10);

        quoteHub.Add(Quotes.Take(50));

        quoteHub.HasObservers.Should().BeTrue();
        emaHub.HasObservers.Should().BeTrue();

        // dispose the middle hub
        emaHub.Dispose();

        // detached from its provider...
        emaHub.IsSubscribed.Should().BeFalse();
        quoteHub.HasObservers.Should().BeFalse();

        // ...and its own subscriber was completed (and unsubscribed itself)
        emaHub.HasObservers.Should().BeFalse();
        smaHub.IsSubscribed.Should().BeFalse();

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void DisposeChain_TearsDownEntireChain()
    {
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        SmaHub smaHub = emaHub.ToSmaHub(10);

        quoteHub.Add(Quotes.Take(50));

        // tear down the whole chain from the root
        quoteHub.DisposeChain();

        quoteHub.HasObservers.Should().BeFalse();
        emaHub.HasObservers.Should().BeFalse();
        emaHub.IsSubscribed.Should().BeFalse();
        smaHub.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void DisposeChain_TearsDownHiddenIntermediateHub()
    {
        // GatorHub hides an internal AlligatorHub as its provider; that hidden
        // hub is the direct observer of the root and must be torn down too.
        QuoteHub quoteHub = new();
        GatorHub gatorHub = quoteHub.ToGatorHub();

        quoteHub.Add(Quotes.Take(50));

        quoteHub.HasObservers.Should().BeTrue();  // the hidden AlligatorHub

        quoteHub.DisposeChain();

        quoteHub.HasObservers.Should().BeFalse();  // hidden hub detached
        gatorHub.IsSubscribed.Should().BeFalse();
    }

    [TestMethod]
    public void Dispose_IsIdempotent()
    {
        QuoteHub quoteHub = new();
        EmaHub emaHub = quoteHub.ToEmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        emaHub.Dispose();

        // a second dispose is a harmless no-op
        Action act = emaHub.Dispose;
        act.Should().NotThrow();

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void DisposeChain_IsIdempotent()
    {
        QuoteHub quoteHub = new();
        _ = quoteHub.ToEmaHub(20).ToSmaHub(10);
        quoteHub.Add(Quotes.Take(50));

        quoteHub.DisposeChain();

        Action act = quoteHub.DisposeChain;
        act.Should().NotThrow();
    }

    [TestMethod]
    public void RootHub_WorksWithUsingBlock()
    {
        // hubs are IDisposable, so a scoped root works with `using`
        using QuoteHub quoteHub = new();

        EmaHub emaHub = quoteHub.ToEmaHub(20);
        quoteHub.Add(Quotes.Take(50));

        emaHub.Results.Should().HaveCount(50);
        // disposed at end of scope — no explicit EndTransmission needed
    }

    [TestMethod]
    public void Dispose_ThrowsObjectDisposedOnSubsequentMutation()
    {
        // any public mutation after Dispose() must throw ObjectDisposedException
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        quoteHub.Dispose();

        Action addOne = () => quoteHub.Add(Quotes.ElementAt(50));
        Action addBatch = () => quoteHub.Add(Quotes.Take(5));
        Action removeAt = () => quoteHub.RemoveAt(0);
        Action removeRange = () => quoteHub.RemoveRange(DateTime.MinValue, notify: false);
        Action subscribe = () => quoteHub.Subscribe(new NoopObserver());
        Action endTx = () => quoteHub.EndTransmission();

        addOne.Should().ThrowExactly<ObjectDisposedException>();
        addBatch.Should().ThrowExactly<ObjectDisposedException>();
        removeAt.Should().ThrowExactly<ObjectDisposedException>();
        removeRange.Should().ThrowExactly<ObjectDisposedException>();
        subscribe.Should().ThrowExactly<ObjectDisposedException>();
        endTx.Should().ThrowExactly<ObjectDisposedException>();
    }

}

/// <summary>
/// Minimal no-op observer for disposal tests.
/// </summary>
file sealed class NoopObserver : IStreamObserver<IQuote>
{
    public bool IsSubscribed => false;
    public void Unsubscribe() { }
    public void OnAdd(IQuote item, bool notify, int? indexHint) { }
    public void OnRebuild(DateTime fromTimestamp) { }
    public void OnPrune(DateTime toTimestamp) { }
    public void OnError(Exception exception) { }
    public void OnCompleted() { }
}
