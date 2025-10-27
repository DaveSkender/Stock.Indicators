namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Connors RSI calculations.
/// </summary>
public interface IConnorsRsi
{
    /// <summary>
    /// Gets the number of periods for the RSI calculation on close prices.
    /// </summary>
    int RsiPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the RSI calculation on streak.
    /// </summary>
    int StreakPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the percent rank calculation.
    /// </summary>
    int RankPeriods { get; }
}
