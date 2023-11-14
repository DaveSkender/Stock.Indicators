using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// TUPLE PROVIDER and OBSERVER (BOILERPLATE)

public abstract class ObsTupleSendTuple
    : TupleProvider, IObserver<(DateTime Date, double Value)>
{
    // fields
    private IDisposable? unsubscriber;

    // PROPERTIES

    internal TupleProvider? TupleSupplier { get; set; }

    // METHODS

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
}
