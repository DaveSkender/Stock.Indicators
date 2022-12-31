namespace Skender.Stock.Indicators;

// QUOTES as REPORTER BASE (BOILERPLATE)

public class QuoteObserver : IObserver<Quote>
{
    // fields
    private IDisposable? unsubscriber;

    // methods
    public virtual void Subscribe(IObservable<Quote> provider)
        => unsubscriber = provider != null
            ? provider.Subscribe(this)
            : throw new ArgumentNullException(nameof(provider));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception ex) => throw ex;

    public virtual void OnNext(Quote value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
