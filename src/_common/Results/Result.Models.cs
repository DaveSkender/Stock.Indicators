namespace Skender.Stock.Indicators;

// RESULT MODELS

public interface IReusableResult : ISeries
{
    public double? Value { get; }
}

[Serializable]
public abstract class ResultBase : ISeries
{
    public DateTime Date { get; set; }
}
