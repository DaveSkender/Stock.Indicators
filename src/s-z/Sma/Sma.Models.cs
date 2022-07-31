namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SmaResult : ResultBase, IReusableResult
{
    public SmaResult(DateTime date)
    {
        Date = date;
    }

    public double? Sma { get; set; }

    double? IReusableResult.Value => Sma;
}

[Serializable]
public sealed class SmaAnalysis : ResultBase, IReusableResult
{
    public SmaAnalysis(DateTime date)
    {
        Date = date;
    }

    public double? Sma { get; set; } // simple moving average
    public double? Mad { get; set; } // mean absolute deviation
    public double? Mse { get; set; } // mean square error
    public double? Mape { get; set; } // mean absolute percentage error

    double? IReusableResult.Value => Sma;
}
