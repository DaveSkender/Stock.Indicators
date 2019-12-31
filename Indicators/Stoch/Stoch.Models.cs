using System;

namespace Skender.Stock.Indicators
{

    public class StochResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public float? Oscillator { get; set; }
        internal float? Smooth { get; set; }
        public float? Signal { get; set; }
        public bool? IsIncreasing { get; set; }
    }

}
