namespace Skender.Stock.Indicators;

// RESULT MODELS

public interface IResult
{
    public DateTime Date { get; }
}

public interface IReusableResult : IResult
{
    internal double? Value { get; }
}

[Serializable]
public abstract class ResultBase : IResult
{
    public DateTime Date { get; set; }
}
