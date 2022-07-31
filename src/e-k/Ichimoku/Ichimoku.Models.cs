namespace Skender.Stock.Indicators;

[Serializable]
public sealed class IchimokuResult : ResultBase
{
    public IchimokuResult(DateTime date)
    {
        Date = date;
    }

    public decimal? TenkanSen { get; set; } // conversion line
    public decimal? KijunSen { get; set; } // base line
    public decimal? SenkouSpanA { get; set; } // leading span A
    public decimal? SenkouSpanB { get; set; } // leading span B
    public decimal? ChikouSpan { get; set; } // lagging span
}
