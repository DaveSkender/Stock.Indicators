[Serializable]
public record ChartThreshold
{
    public double Value { get; }
    public string Color { get; }
    public string Style { get; }
    public ChartFill? Fill { get; }
}
