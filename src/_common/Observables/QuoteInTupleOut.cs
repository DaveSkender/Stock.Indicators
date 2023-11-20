using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSERVE QUOTE SEND TUPLE (BOILERPLATE)

public abstract class QuoteInTupleOut<TQuote> : TupleProvider,
    IObserver<(Act act, TQuote quote)>
    where TQuote : IQuote, new()
{
    // fields
    private IDisposable? unsubscriber;

    // constructor (default, unmanaged)
    protected QuoteInTupleOut()
    {
        QuoteSupplier = new();
    }

    // PROPERTIES

    internal QuoteProvider<TQuote> QuoteSupplier { get; set; }

    // METHODS

    public virtual void Subscribe()
        => unsubscriber = QuoteSupplier != null
            ? QuoteSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(QuoteSupplier));

    [ExcludeFromCodeCoverage]
    public virtual void OnNext((Act act, TQuote quote) value)
    {
        // » handle new quote with override in observer

        // TODO: add generic TResult hander, without override
    }

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
