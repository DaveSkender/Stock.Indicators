namespace Skender.Stock.Indicators
{

    public class StochResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Signal { get; set; }
        public bool? IsIncreasing { get; set; }

        // internal use only
        internal decimal? Smooth { get; set; }
    }

}
