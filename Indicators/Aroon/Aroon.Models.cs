using System;

namespace Skender.Stock.Indicators
{

    public class AroonResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? AroonUp { get; set; }
        public decimal? AroonDown { get; set; }
        public decimal? Oscillator { get; set; }
    }

}
