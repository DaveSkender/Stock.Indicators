using System;

namespace Skender.Stock.Indicators
{

    public class ParabolicSarResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Sar { get; set; }
        public bool? IsReversal { get; set; }
        public bool? IsRising { get; set; }
    }

}
