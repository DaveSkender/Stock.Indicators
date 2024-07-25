
namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER BASE)

# region hub observer variants

public abstract class QuoteObserver<TIn, TOut>(
    IQuoteProvider<TIn> provider
) : StreamHub<TIn, TOut>(provider)
    where TIn : IQuote
    where TOut : ISeries;

public abstract class ReusableObserver<TIn, TOut>(
    IChainProvider<TIn> provider
) : StreamHub<TIn, TOut>(provider)
    where TIn : IReusable
    where TOut : ISeries;
#endregion

/// <summary>
/// Streaming hub (abstract observer/provider)
/// </summary>
public abstract partial class StreamHub<TIn, TOut>(
    IStreamProvider<TIn> provider
) : StreamProvider<TOut>, IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected internal IDisposable? Subscription { get; set; }

    protected IStreamProvider<TIn> Provider => provider;

    // observer methods

    public virtual void OnNext((Act, TIn, int?) value)
    {
        (Act act, TIn item, int? index) = value;

        if (act is Act.Unknown or Act.AddNew)
        {
            Add(act, item, index);
            return;
        }

        // TODO: handle revision/recursion differently
        // for different indicators; and may also need
        // to breakout OnDeleted(TIn deleted), etc.
        RebuildCache(item.Timestamp);
    }

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    public abstract override string ToString();
}
