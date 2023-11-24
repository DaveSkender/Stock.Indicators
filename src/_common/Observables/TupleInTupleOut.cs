namespace Skender.Stock.Indicators;

// TUPLE PROVIDER and OBSERVER (BOILERPLATE)

public abstract class TupleInTupleOut : TupleProvider,
    IObserver<(Act, DateTime, double)>
{
    // fields
    private IDisposable? unsubscriber;

    // constructor (default, unmanaged)
    protected TupleInTupleOut()
    {
        TupleSupplier = new();
    }

    // PROPERTIES

    internal TupleProvider TupleSupplier { get; set; }

    // METHODS

    public virtual void Subscribe()
        => unsubscriber = TupleSupplier != null
            ? TupleSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(TupleSupplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    // TODO: add generic TResult hander, without override
    public virtual void OnNext((Act, DateTime, double) value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
