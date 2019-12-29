using System;

namespace StockIndicators
{

    public class StochResult
    {
        public DateTime Date { get; internal set; }
        internal int Index { get; set; }
        public float? Oscillator { get; internal set; }
        internal float? Smooth { get; set; }
        public float? Signal { get; internal set; }
        public bool? IsIncreasing { get; internal set; }
    }

    public enum StochType
    {
        SLOW,
        FAST,
        FULL
    }

}
