namespace Skender.Stock.Indicators;

public interface ISmaResult : IResult
{
    public double? Sma { get; }
}

[Serializable]
public sealed class SmaResult : ResultBase, ISmaResult, IReusableResult
{
    public double? Sma { get; set; }

    double? IReusableResult.Value => Sma;
}

[Serializable]
public class SmaExtendedResult : ResultBase, ISmaResult
{
    public double? Sma { get; set; } // simple moving average
    public double? Mad { get; set; } // mean absolute deviation
    public double? Mse { get; set; } // mean square error
    public double? Mape { get; set; } // mean absolute percentage error
}
