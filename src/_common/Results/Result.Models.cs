namespace Skender.Stock.Indicators;

// RESULT MODELS

/// <summary>
/// Interface for chainable, resusable indicator results.
/// </summary>
public interface IReusableResult : ISeries
{
    /// <summary>
    /// Selected value that propogates for chainable indicators.
    /// It is consumed by chainee indicators for their own purposes.
    /// </summary>
    double? Value { get; }
}

/// <summary>
/// Base elements used across all indicator results.
/// </summary>
public abstract class ResultBase : ISeries
{
    /// <inheritdoc/>
    public DateTime Date { get; set; }
}
