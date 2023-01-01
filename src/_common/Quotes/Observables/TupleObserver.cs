namespace Skender.Stock.Indicators;

// OBSERVER of TUPLES (BOILERPLATE)

public abstract class TupleObserver : IObserver<(DateTime Date, double Value)>
{
    // fields
    private IDisposable? unsubscriber;

    // properites
    public TupleProvider? Provider { get; set; }

    // methods
    public virtual void Subscribe()
        => unsubscriber = Provider != null
            ? Provider.Subscribe(this)
            : throw new ArgumentNullException(nameof(Provider));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    public virtual void OnNext((DateTime Date, double Value) value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
