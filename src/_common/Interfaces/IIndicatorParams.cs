namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for common indicator parameters.
/// </summary>
public interface IIndicatorParams
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}

/// <summary>
/// Interface for single-period moving average indicators.
/// </summary>
public interface ISinglePeriodIndicator : IIndicatorParams
{
    // Marker interface for single-period indicators like SMA, EMA
}

/// <summary>
/// Interface for exponential smoothing indicators.
/// </summary>
public interface IExponentialIndicator : ISinglePeriodIndicator
{
    /// <summary>
    /// Gets the smoothing factor for the calculation.
    /// </summary>
    double K { get; }
}

/// <summary>
/// Interface for multi-period indicators like MACD.
/// </summary>
public interface IMultiPeriodIndicator : IIndicatorParams
{
    /// <summary>
    /// Gets the number of fast periods.
    /// </summary>
    int FastPeriods { get; }

    /// <summary>
    /// Gets the number of slow periods.
    /// </summary>
    int SlowPeriods { get; }

    /// <summary>
    /// Gets the number of signal periods.
    /// </summary>
    int SignalPeriods { get; }
}