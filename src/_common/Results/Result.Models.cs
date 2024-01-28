namespace Skender.Stock.Indicators;

// RESULT MODELS

// Cosmetic alias for ISeries,
// to clarify indicator use intention (readability)
public interface IResult : ISeries;

// result that can provide a single chainable value
public interface IReusableResult : IResult
{
    double Value { get; }
}

// base result
public abstract class ResultBase : IResult
{
    public DateTime Date { get; set; }
}
