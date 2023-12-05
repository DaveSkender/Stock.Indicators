namespace Skender.Stock.Indicators;

public abstract class ChainObserver<TResult> : SeriesCache<TResult>,
    IObserver<(Act act, DateTime date, double price)>
    where TResult : ISeries, new()
{
    internal IDisposable? unsubscriber;

    internal ChainObserver(ChainProvider provider)
    {
        ChainSupplier = provider;
    }

    // PROPERTIES

    internal ChainProvider ChainSupplier { get; }

    // METHODS

    // standard observer properties

    public abstract void OnNext((Act act, DateTime date, double price) value);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    public void Initialize()
    {
        ClearCache();  // delete cache entries

        // nothing to do
        if (ChainSupplier.Chain.Count == 0)
        {
            return;
        }

        // rebuild from date
        DateTime fromDate = ChainSupplier.Chain[0].Date;
        RebuildCache(fromDate);
    }

    // replay from supplier cache
    internal void RebuildCache(DateTime fromDate)
    {
        int startIndex = ChainSupplier.Chain.FindIndex(fromDate);

        if (startIndex == -1)
        {
            throw new InvalidOperationException("Cache rebuild starting target not found.");
        }

        for (int i = startIndex; i < ChainSupplier.Chain.Count; i++)
        {
            (DateTime date, double value) = ChainSupplier.Chain[i];
            OnNext((Act.AddNew, date, value));
        }
    }

    // delete cache entries, gracefully (and optionally, notifies observers)
    internal void ClearCache()
    {
        // nothing to do
        if (Cache.Count == 0)
        {
            Cache = [];
            Chain = [];
            return;
        }

        // reset from date
        DateTime fromDate = Chain[0].Date;
        ClearCache(fromDate);
    }

    // delete cache entries after fromDate (inclusive)
    internal void ClearCache(DateTime fromDate)
    {
        // index range
        int s = Cache.FindIndex(fromDate);
        int e = Cache.Count - 1;

        if (s == -1)
        {
            throw new InvalidOperationException("Cache clear starting target not found.");
        }

        ClearCache(s, e);
    }

    // delete cache entries between index range values (implemented in inheritor)
    internal abstract void ClearCache(int fromIndex, int toIndex);
}
