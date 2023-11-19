using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSERVER of QUOTES (BOILERPLATE)

public abstract class QuoteIn<TQuote>
    : IObserver<(Act, TQuote)>
    where TQuote : IQuote, new()
{
    // fields
    private IDisposable? unsubscriber;

    // constructor (default, unmanaged)
    protected QuoteIn()
    {
        QuoteSupplier = new();
    }

    // properties
    internal QuoteProvider<TQuote> QuoteSupplier { get; set; }

    // methods
    public virtual void Subscribe()
        => unsubscriber = QuoteSupplier != null
            ? QuoteSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(QuoteSupplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    [ExcludeFromCodeCoverage]
    public virtual void OnNext((Act, TQuote) value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
