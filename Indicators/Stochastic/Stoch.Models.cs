namespace Skender.Stock.Indicators
{

    public class StochResult : ResultBase
    {
        public float? Oscillator { get; set; }
        internal float? Smooth { get; set; }
        public float? Signal { get; set; }
        public bool? IsIncreasing { get; set; }
    }

}
