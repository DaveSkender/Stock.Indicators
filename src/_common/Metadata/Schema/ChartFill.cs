[Serializable]
public record ChartFill
{
    public string Target { get; init; }
    public string ColorAbove { get; init; }
    public string ColorBelow { get; init; }
}
