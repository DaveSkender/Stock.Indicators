namespace Skender.Stock.Indicators
{

    public class ParabolicSarResult : ResultBase
    {
        public decimal? Sar { get; set; }
        public bool? IsReversal { get; set; }
        public bool? IsRising { get; set; }
    }

}
