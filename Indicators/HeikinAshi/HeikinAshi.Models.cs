namespace Skender.Stock.Indicators
{

    public class HeikinAshiResult : ResultBase
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public bool? IsBullish { get; set; }
        public decimal Weakness { get; set; }
    }

}
