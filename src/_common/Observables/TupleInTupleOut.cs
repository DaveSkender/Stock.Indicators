using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// TUPLE PROVIDER and OBSERVER (BOILERPLATE)

public abstract class TupleInTupleOut : TupleProvider,
    IObserver<(Disposition, DateTime, double)>
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

    [ExcludeFromCodeCoverage]
    public virtual void OnNext((Disposition, DateTime, double) value)
    {
        // » overrided with custom handler in instantiated class
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
