using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSERVER of TUPLES (BOILERPLATE)

public abstract class TupleObserver
    : IObserver<(Act act, DateTime Date, double Value)>
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

    // methods
    public virtual void Subscribe()
        => unsubscriber = TupleSupplier != null
            ? TupleSupplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(TupleSupplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    [ExcludeFromCodeCoverage]
    public virtual void OnNext((DateTime Date, double Value) value)
    {
        // » overrided with custom handler in instantiated class
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
    public void OnNext((Act act, DateTime Date, double Value) value) => throw new NotImplementedException();
}
