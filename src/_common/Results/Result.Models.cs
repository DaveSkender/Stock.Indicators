namespace Skender.Stock.Indicators;

// RESULT MODELS
public interface IResult : ISeries { }

// TODO: make non-nullable, see EMA increment for rationale and better resilience
// use of SyncIndex may not be needed, especially for Streaming
// though, keep in mind, replacing with IResultNaN below means carrying two sets of data
// and may be worse performance, testing needed

public interface IReusableResult : IResult
{
    public double Value { get; }
}

[Serializable]
public abstract class ResultBase : IResult
{
    public DateTime Date { get; set; }
}
