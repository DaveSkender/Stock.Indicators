
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
    IStreamObservable<TIn> provider
) : StreamProvider<TOut>, IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected internal IDisposable? Subscription { get; set; }

    protected IStreamObservable<TIn> Provider => provider;

    // observer methods

    public virtual void OnNext((Act, TIn, int?) value)
    {
        (Act act, TIn item, int? index) = value;

        // analyze (pre-check)
        if (act is Act.Unknown)
        {
            act = CheckSequence(item.Timestamp);
        }

        // pass-thru to implementation
        if (act is Act.Add)
        {
            Add(item, index);
            return;
        }

        // should only be rebuild at this point
        if (act is Act.Rebuild)
        {
            throw new InvalidOperationException("Invalid action type.");
        }

        // rebuild from provider
        RebuildCache(item.Timestamp);
    }

    public void OnError(Exception exception) => throw exception;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    public abstract override string ToString();
}
