using System;

namespace Skender.Stock.Indicators
{

    public class RsiResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        internal float Gain { get; set; } = 0;
        internal float Loss { get; set; } = 0;
        public float? Rsi { get; set; }
        public bool? IsIncreasing { get; set; }
    }

}
