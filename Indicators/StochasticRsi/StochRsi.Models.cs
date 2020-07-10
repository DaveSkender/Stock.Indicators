namespace Skender.Stock.Indicators
{

    public class StochRsiResult : ResultBase
    {
        public decimal? StochRsi { get; set; }
        public decimal? Signal { get; set; }
        public bool? IsIncreasing { get; set; }
    }

}
