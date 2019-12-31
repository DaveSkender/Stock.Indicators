using System;

namespace Skender.Stock.Indicators
{

    public class MacdResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Macd { get; set; }
        public decimal? Signal { get; set; }
        public decimal? Histogram { get; set; }
        public bool? IsBullish { get; set; }
        public bool? IsDiverging { get; set; }
    }

}
