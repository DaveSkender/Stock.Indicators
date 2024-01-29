namespace Skender.Stock.Indicators;

public sealed record class IchimokuResult : IResult
{
    public DateTime TickDate { get; set; }
    public decimal? TenkanSen { get; set; } // conversion line
    public decimal? KijunSen { get; set; } // base line
    public decimal? SenkouSpanA { get; set; } // leading span A
    public decimal? SenkouSpanB { get; set; } // leading span B
    public decimal? ChikouSpan { get; set; } // lagging span
}
