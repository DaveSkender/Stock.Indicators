namespace Skender.Stock.Indicators;

/// <summary>
/// ISeries is the base interface for all of our time-series enumerables,
/// notable for quotes and indicator results.
/// </summary>
public interface ISeries
{
    /// <summary>
    /// Date in the time-series
    /// </summary>
    DateTime Date { get; }
}
