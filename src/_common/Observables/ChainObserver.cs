namespace Skender.Stock.Indicators;

// CHAIN OBSERVER

public abstract class ChainObserver<TResult>
    : SeriesCache<TResult>, IChainObserver<TResult>
    where TResult : IResult, new()
{
    internal IDisposable? unsubscriber;

    private protected ChainObserver(ChainProvider provider)
    {
        ChainSupplier = provider;
    }

    // PROPERTIES

    internal ChainProvider ChainSupplier { get; }

    // METHODS

    public abstract void OnNext((Act act, DateTime date, double price) value);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    public void RebuildCache()
    {
        // nothing to do
        if (ChainSupplier.Chain.Count == 0)
        {
            return;
        }

        // rebuild
        RebuildCache(0);
    }

    // replay from supplier cache, from date
    public void RebuildCache(DateTime fromTimestamp)
        => RebuildCache(fromTimestamp, 0);

    internal override void RebuildCache(
        DateTime fromTimestamp, int offset = 0)
    {
        int fromIndex = ChainSupplier.Chain.FindIndex(fromTimestamp);

        if (fromIndex == -1)
        {
            throw new InvalidOperationException(
                "Cache rebuild starting target not found.");
        }

        RebuildCache(fromIndex, offset);
    }

    // replay from supplier cache, from index
    internal override void RebuildCache(
        int fromIndex, int offset = 0)
    {
        int firstIndex = fromIndex + offset;

        // clear forward values
        ClearCache(firstIndex);

        // replay from source
        for (int i = firstIndex; i < ChainSupplier.Chain.Count; i++)
        {
            (DateTime date, double value) = ChainSupplier.Chain[i];
            OnNext((Act.AddNew, date, value));
        }
    }
}
