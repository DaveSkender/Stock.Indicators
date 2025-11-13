namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Standard Deviation (StdDev) calculations.
/// </summary>
public interface IStdDev
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}
