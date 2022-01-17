namespace Skender.Stock.Indicators;

// CUSTOMIZABLE SETTINGS
public partial class Indicator
{
    public static void UseConfig(IndicatorConfig configuration)
    {
        config = configuration;
    }
}

public class IndicatorConfig
{
    public bool UseBadQuotesException { get; set; } = true;
}