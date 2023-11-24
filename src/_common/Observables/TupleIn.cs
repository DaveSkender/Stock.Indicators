namespace Skender.Stock.Indicators;

// OBSERVER of TUPLES (BOILERPLATE)

public abstract class TupleObserver
    : IObserver<(Act, DateTime, double)>
{
    // fields
    private IDisposable? unsubscriber;

    // constructor (default, unmanaged)
    protected TupleObserver()
    {
        TupleSupplier = new();
    }

    // properites
    internal TupleProvider TupleSupplier { get; set; }

    // TODO: add generic TResult storage

    // methods
    public virtual void Subscribe()
        => unsubscriber = TupleSupplier != null
            ? TupleSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(TupleSupplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    // TODO: add generic TResult hander, without override
    public virtual void OnNext((Act, DateTime, double) value) => throw new NotImplementedException();

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
