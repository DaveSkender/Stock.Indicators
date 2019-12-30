using System;

namespace Skender.Stock.Indicators
{

    public class ParabolicSarResult
    {
        public DateTime Date { get; internal set; }
        public decimal? Sar { get; internal set; }
        public bool? IsReversal { get; internal set; }
        public bool? IsBullish { get; internal set; }
    }

}
