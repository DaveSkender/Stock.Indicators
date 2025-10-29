namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Standard Deviation (StdDev) hub.
/// </summary>
public interface IStdDev
{
    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    int LookbackPeriods { get; }
}
