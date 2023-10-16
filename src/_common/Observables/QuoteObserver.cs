namespace Skender.Stock.Indicators;

// OBSERVER of QUOTES (BOILERPLATE)

public abstract class QuoteObserver : IObserver<Quote>
{
    // fields
    private IDisposable? unsubscriber;

    // properties
    internal QuoteProvider? Supplier { get; set; }

    // methods
    public virtual void Subscribe()
        => unsubscriber = Supplier != null
            ? Supplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(Supplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    public virtual void OnNext(Quote value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
