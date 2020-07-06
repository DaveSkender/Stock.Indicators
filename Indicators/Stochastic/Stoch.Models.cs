namespace Skender.Stock.Indicators
{

    public class StochResult : ResultBase
    {
        public float? Oscillator { get; set; }
        public float? Signal { get; set; }
        public bool? IsIncreasing { get; set; }

        // internal use only
        internal float? Smooth { get; set; }
    }

}
