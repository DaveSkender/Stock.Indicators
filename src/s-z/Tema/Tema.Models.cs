namespace Skender.Stock.Indicators;

public interface ITemaResult : IResult
{
    public double? Tema { get; }
}

[Serializable]
public sealed class TemaResult : ResultBase, ITemaResult, IReusableResult
{
    public double? Tema { get; set; }

    double? IReusableResult.Value => Tema;
}
