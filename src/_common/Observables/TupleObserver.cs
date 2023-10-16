using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSERVER of TUPLES (BOILERPLATE)

public abstract class TupleObserver : IObserver<(DateTime Date, double Value)>
{
    // fields
    private IDisposable? unsubscriber;

    // properites
    internal TupleProvider? Supplier { get; set; }

    // methods
    public virtual void Subscribe()
        => unsubscriber = Supplier != null
            ? Supplier.Subscribe(this)
            : throw new ArgumentNullException(nameof(Supplier));

    public virtual void OnCompleted() => Unsubscribe();

    public virtual void OnError(Exception error) => throw error;

    [ExcludeFromCodeCoverage]
    public virtual void OnNext((DateTime Date, double Value) value)
    {
        // » handle new quote with override in observer
    }

    public virtual void Unsubscribe() => unsubscriber?.Dispose();
}
