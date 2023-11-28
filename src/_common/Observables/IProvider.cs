namespace Skender.Stock.Indicators;

public interface IProvider<TSeries> : IResultCache<TSeries>
    where TSeries : ISeries, new()
{
    // provider contains additional overflow and other common provider (observable) controls

    // PROPERTIES

    public int OverflowCount { get; internal set; }

    public TSeries LastArrival { get; internal set; }
}

public interface IResultCache<TSeries>
    where TSeries : ISeries, new()
{
    // general indicator result cache

    // PROPERTIES

    public IEnumerable<TSeries> Results { get; }

    internal List<TSeries> Cache { get; set; }

    // METHODS

    internal abstract void ResetCache();
}
