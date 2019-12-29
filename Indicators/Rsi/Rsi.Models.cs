using System;

namespace StockIndicators
{

    public class RsiResult
    {
        public DateTime Date { get; internal set; }
        internal int Index { get; set; }
        internal float Gain { get; set; } = 0;
        internal float Loss { get; set; } = 0;
        public float? Rsi { get; internal set; }
        public bool? IsIncreasing { get; internal set; }
    }

}
