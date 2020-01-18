using System;

namespace Skender.Stock.Indicators
{

    public class StochRsiResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public float? StochRsi { get; set; }
        public bool? IsIncreasing { get; set; }
    }

}
