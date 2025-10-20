namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Detrended Price Oscillator (DPO) calculations.
/// </summary>
public interface IDpo
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}
