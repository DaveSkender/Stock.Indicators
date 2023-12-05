namespace Skender.Stock.Indicators;

// RESULT MODELS

// result that cannot be converted to a single chainable value.
// This is a cosmetic alias for ISeries, to clarify intent (readability)
public interface IResult : ISeries;

// result that can provide a single chainable value
// TODO: will this be more usable if it were an abstract class and property?
public interface IReusableResult : IResult
{
    public double Value { get; }
}

// base result
[Serializable]
public abstract class ResultBase : IResult
{
    public DateTime Date { get; set; }
}
