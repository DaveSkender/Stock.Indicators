using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSERVE QUOTE SEND TUPLE (BOILERPLATE)
// TODO: convert to TQuote
public abstract class ObsQuoteSendTuple : TupleProvider, IObserver<Quote>
{
    // fields
    private IDisposable? unsubscriber;

    // constructor (default, unmanaged)
    protected ObsQuoteSendTuple()
    {
        QuoteSupplier = new();
    }

    // PROPERTIES

    internal QuoteProvider QuoteSupplier { get; set; }

    // METHODS

    public virtual void Subscribe()
        => unsubscriber = QuoteSupplier != null
            ? QuoteSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(QuoteSupplier));

    [ExcludeFromCodeCoverage]
    public virtual void OnNext(Quote value)
    {
        // » handle new quote with override in observer
    }

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
