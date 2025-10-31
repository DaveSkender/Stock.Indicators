namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Slope and Linear Regression calculations.
/// </summary>
public interface ISlope
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}
