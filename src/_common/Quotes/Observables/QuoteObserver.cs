namespace Skender.Stock.Indicators;

// OBSERVER of QUOTES (BOILERPLATE)

public abstract class QuoteObserver : IObserver<Quote>
{
    // fields
    private IDisposable? unsubscriber;

    // properites
    public QuoteProvider? Provider { get; set; }

    // methods
    public virtual void Subscribe(IObservable<Quote> provider)
        => unsubscriber = provider != null
            ? provider.Subscribe(this)
            : throw new ArgumentNullException(nameof(provider));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    public virtual void OnNext(Quote value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
