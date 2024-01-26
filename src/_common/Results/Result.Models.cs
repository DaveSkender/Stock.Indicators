using System.Text.Json.Serialization;

namespace Skender.Stock.Indicators;

// RESULT MODELS

public interface IReusableResult : ISeries
{
    public double? Value { get; }
}

[Serializable]
public abstract class ResultBase : ISeries
{
    [JsonPropertyOrder(-1)]
    public DateTime Date { get; set; }
}
