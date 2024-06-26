namespace Skender.Stock.Indicators;

// RESULT INTERFACES

/// <summary>
/// Cosmetic alias for ISeries, to uniquely
/// identify non-chainable indicator results.
/// </summary>
public interface IResult : ISeries;

/// <summary>
/// Indicator result type that identifies
/// a single chainable value.
/// </summary>
public interface IReusableResult : IResult
{
    /// <summary>
    /// Result value that is passed to chained indicators.
    /// </summary>
    double Value { get; }
}
